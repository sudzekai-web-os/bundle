using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Mapping
{
    public static unsafe class AssemblyVTableMapper
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LogCallback(LogEntryDtoDescriptor* entry);

        public static AssemblyVTable ToManaged(AssemblyVTableDescriptor descriptor)
        {
            if (descriptor.AbiVersion != LinCtrlContext.AbiVersion)
                throw new InvalidOperationException($"Unsupported ABI version: {descriptor.AbiVersion}");
            if (descriptor.InitDelegate == null || descriptor.BuildDelegate == null ||
                descriptor.DelegateLoggingDelegate == null || descriptor.GetRequestHandlersDelegate == null ||
                descriptor.GetRequestHandlersMetadataDelegate == null || descriptor.DisposeAssemblyDelegate == null)
                throw new InvalidOperationException("Assembly virtual table is incomplete");

            var session = new AssemblyVTableSession(descriptor);
            return new()
            {
                AbiVersion = descriptor.AbiVersion,
                InitDelegate = session.Initialize,
                BuildDelegate = session.Build,
                DelegateLoggingDelegate = session.DelegateLogging,
                GetRequestHandlersDelegate = session.GetHandlers,
                GetRequestHandlersMetadataDelegate = session.GetMetadata,
                AssemblyDisposeDelegate = session.Dispose
            };
        }

        internal static void ThrowIfFailed(int result)
        {
            if (result != 0)
                throw new InvalidOperationException($"Native callback failed with result code: {result}");
        }

        private sealed class AssemblyVTableSession(AssemblyVTableDescriptor descriptor)
        {
            private readonly ReaderWriterLockSlim _sync = new();
            private AssemblyVTableDescriptor _descriptor = descriptor;
            private LogCallback? _logCallback;
            private bool _disposed;

            private void Enter()
            {
                _sync.EnterReadLock();
                if (_disposed)
                {
                    _sync.ExitReadLock();
                    throw new ObjectDisposedException(nameof(AssemblyVTableSession));
                }
            }

            public void Initialize()
            {
                Enter();
                try
                {
                    ThrowIfFailed(_descriptor.InitDelegate());
                }
                finally
                {
                    _sync.ExitReadLock();
                }
            }

            public void Build()
            {
                Enter();
                try
                {
                    ThrowIfFailed(_descriptor.BuildDelegate());
                }
                finally
                {
                    _sync.ExitReadLock();
                }
            }

            public void DelegateLogging(LogDelegate log)
            {
                ArgumentNullException.ThrowIfNull(log);
                _sync.EnterWriteLock();
                try
                {
                    ObjectDisposedException.ThrowIf(_disposed, this);
                    if (_logCallback != null)
                        throw new InvalidOperationException("Logging already delegated");

                    _logCallback = entry =>
                    {
                        try
                        {
                            if (entry == null)
                                return 1;
                            log(new()
                            {
                                Category = StringMapper.ToManaged(entry->Category),
                                Message = StringMapper.ToManaged(entry->Message),
                                LogLevel = (Microsoft.Extensions.Logging.LogLevel)entry->LogLevel
                            });
                            return 0;
                        }
                        catch
                        {
                            return 1;
                        }
                    };
                    ThrowIfFailed(_descriptor.DelegateLoggingDelegate(
                        (delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int>)Marshal.GetFunctionPointerForDelegate(_logCallback)));
                }
                finally
                {
                    _sync.ExitWriteLock();
                }
            }

            public IRequestHandler[] GetHandlers()
            {
                Enter();
                RequestHandlerArrayDescriptor handlers = default;
                try
                {
                    ThrowIfFailed(_descriptor.GetRequestHandlersDelegate(&handlers));
                    var result = new IRequestHandler[handlers.Count];
                    for (int i = 0; i < handlers.Count; i++)
                        result[i] = new NativeRequestHandler(this, handlers.Array[i]);
                    return result;
                }
                finally
                {
                    for (int i = 0; i < handlers.Count; i++)
                        RequestHandlerAllocator.Free(ref handlers.Array[i]);
                    var array = handlers.Array;
                    CoTaskAllocator.Free(ref array);
                    _sync.ExitReadLock();
                }
            }

            public string GetMetadata()
            {
                Enter();
                StringViewer metadata = default;
                try
                {
                    ThrowIfFailed(_descriptor.GetRequestHandlersMetadataDelegate(&metadata));
                    return StringMapper.ToManaged(metadata);
                }
                finally
                {
                    StringAllocator.Free(ref metadata);
                    _sync.ExitReadLock();
                }
            }

            public HttpResponseDto Handle(
                delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int> handle,
                HttpRequestDto request)
            {
                Enter();
                HttpRequestDtoDescriptor requestDescriptor = default;
                HttpResponseDtoDescriptor responseDescriptor = default;
                try
                {
                    requestDescriptor = HttpDtoMapper.ToDescriptor(request);
                    ThrowIfFailed(handle(&requestDescriptor, &responseDescriptor));
                    return HttpDtoMapper.ToManaged(responseDescriptor);
                }
                finally
                {
                    HttpRequestDtoAllocator.Free(ref requestDescriptor);
                    HttpResponseDtoAllocator.Free(ref responseDescriptor);
                    _sync.ExitReadLock();
                }
            }

            public void Dispose()
            {
                _sync.EnterWriteLock();
                try
                {
                    if (_disposed)
                        return;

                    _disposed = true;
                    try
                    {
                        ThrowIfFailed(_descriptor.DisposeAssemblyDelegate());
                    }
                    finally
                    {
                        GC.KeepAlive(_logCallback);
                        _logCallback = null;
                        _descriptor = default;
                    }
                }
                finally
                {
                    _sync.ExitWriteLock();
                }
            }
        }

        private sealed class NativeRequestHandler : IRequestHandler
        {
            private readonly AssemblyVTableSession _session;
            private readonly delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int> _handle;

            public NativeRequestHandler(AssemblyVTableSession session, RequestHandlerDescriptor descriptor)
            {
                if (descriptor.HandleDelegate == null)
                    throw new InvalidOperationException("Request handler callback was null");

                _session = session;
                Pattern = StringMapper.ToManaged(descriptor.Pattern);
                HttpMethod = (HttpRequestMethod)descriptor.HttpMethod;
                IsFiltering = descriptor.IsFiltering != 0;
                _handle = descriptor.HandleDelegate;
            }

            public string Pattern { get; }

            public HttpRequestMethod HttpMethod { get; }

            public bool IsFiltering { get; }

            public HttpResponseDto Handle(HttpRequestDto request)
                => _session.Handle(_handle, request);
        }
    }
}
