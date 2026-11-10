using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Mapping
{
    // Registration roots callbacks for the lifetime of the plugin.
    // Returned descriptors own CoTaskMem buffers; the caller frees them after copying.
    public sealed unsafe class AssemblyVTableRegistration
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int ActionCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LoggingCallback(delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int> log);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int HandlersCallback(RequestHandlerArrayDescriptor* handlers);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MetadataCallback(StringViewer* metadata);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int HandleCallback(HttpRequestDtoDescriptor* request, HttpResponseDtoDescriptor* response);

        private readonly object _sync = new();
        private IRequestHandler[]? _requestHandlers;
        private readonly List<Delegate> _callbacks = [];
        private readonly Dictionary<IRequestHandler, HandleCallback> _handlers = [];
        private AssemblyVTable _table;
        private bool _created;
        private bool _disposed;

        public bool IsDisposed => _disposed;

        public AssemblyVTableRegistration(AssemblyVTable table)
        {
            _table = table;
        }

        private nint Register<T>(T callback)
            where T : Delegate
        {
            _callbacks.Add(callback);
            return Marshal.GetFunctionPointerForDelegate(callback);
        }

        public AssemblyVTableDescriptor ToDescriptor()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_created)
                throw new InvalidOperationException("Assembly virtual table already created");

            _created = true;
            return AssemblyVTableAllocator.Alloc(_table.AbiVersion,
                (delegate* unmanaged[Cdecl]<int>)Register(new ActionCallback(() => Invoke(_table.InitDelegate.Invoke))),
                (delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int>, int>)Register(new LoggingCallback(DelegateLogging)),
                (delegate* unmanaged[Cdecl]<int>)Register(new ActionCallback(() => Invoke(_table.BuildDelegate.Invoke))),
                (delegate* unmanaged[Cdecl]<RequestHandlerArrayDescriptor*, int>)Register(new HandlersCallback(GetHandlers)),
                (delegate* unmanaged[Cdecl]<StringViewer*, int>)Register(new MetadataCallback(GetMetadata)),
                (delegate* unmanaged[Cdecl]<int>)Register(new ActionCallback(DisposeAssembly)));
        }

        private int DisposeAssembly()
        {
            if (_disposed)
                return 0;

            var callbacks = _callbacks.ToArray();
            var handlers = _handlers.Values.ToArray();
            try
            {
                return Invoke(_table.AssemblyDisposeDelegate.Invoke);
            }
            finally
            {
                _disposed = true;
                _handlers.Clear();
                _requestHandlers = null;
                _table = new();
                GC.KeepAlive(handlers);
                GC.KeepAlive(callbacks);
            }
        }

        private static int Invoke(Action action)
        {
            try
            {
                action();
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private int DelegateLogging(delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int> log)
        {
            if (log == null)
                return 1;
            return Invoke(() => _table.DelegateLoggingDelegate(entry =>
            {
                var descriptor = LogEntryDtoAllocator.Alloc(entry.Category, entry.Message, (int)entry.LogLevel);
                try
                {
                    AssemblyVTableMapper.ThrowIfFailed(log(&descriptor));
                }
                finally
                {
                    StringAllocator.Free(ref descriptor.Category);
                    StringAllocator.Free(ref descriptor.Message);
                }
            }));
        }

        private int GetMetadata(StringViewer* metadata)
        {
            if (metadata == null)
                return 1;
            *metadata = default;
            try
            {
                *metadata = StringAllocator.Alloc(_table.GetRequestHandlersMetadataDelegate());
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private int GetHandlers(RequestHandlerArrayDescriptor* result)
        {
            lock (_sync)
                return GetHandlersCore(result);
        }

        private int GetHandlersCore(RequestHandlerArrayDescriptor* result)
        {
            if (result == null)
                return 1;
            *result = default;
            RequestHandlerArrayDescriptor descriptor = default;
            try
            {
                var handlers = _requestHandlers ??= _table.GetRequestHandlersDelegate();
                descriptor.Array = (RequestHandlerDescriptor*)Marshal.AllocCoTaskMem(checked(handlers.Length * sizeof(RequestHandlerDescriptor)));
                foreach (var handler in handlers)
                {
                    if (!_handlers.TryGetValue(handler, out var callback))
                    {
                        callback = (request, response) =>
                        {
                            if (request == null || response == null)
                                return 1;
                            *response = default;
                            try
                            {
                                *response = HttpDtoMapper.ToDescriptor(handler.Handle(HttpDtoMapper.ToManaged(*request)));
                                return 0;
                            }
                            catch
                            {
                                return 1;
                            }
                        };
                        _handlers.Add(handler, callback);
                    }
                    var pattern = StringAllocator.Alloc(handler.Pattern);
                    try
                    {
                        descriptor.Array[descriptor.Count] = RequestHandlerAllocator.Alloc(pattern, (int)handler.HttpMethod,
                            (delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int>)Marshal.GetFunctionPointerForDelegate(callback));
                        descriptor.Count++;
                    }
                    catch
                    {
                        StringAllocator.Free(ref pattern);
                        throw;
                    }
                }
                *result = descriptor;
                return 0;
            }
            catch
            {
                for (int i = 0; i < descriptor.Count; i++)
                    RequestHandlerAllocator.Free(ref descriptor.Array[i]);
                var array = descriptor.Array;
                CoTaskAllocator.Free(ref array);
                return 1;
            }
        }
    }
}
