using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal unsafe class PluginLoader
    {
        delegate void LogDelegate(LogEntryDescriptor descriptor);
        private readonly ILogger<PluginLoader> _logger;
        private readonly GCHandle _handle;
        private readonly nint _logDelegatePtr;

        public PluginLoader(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<PluginLoader>>();

            LogDelegate logAction = (LogEntryDescriptor descriptor) =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                var category = AllocationUtilities.ToString(descriptor.Category, descriptor.CategoryLength);

                var logger = loggerFactory.CreateLogger(category);

                var message = AllocationUtilities.ToString(descriptor.Message, descriptor.MessageLength);

                logger.Log((LogLevel)descriptor.LogLevel, message);

                descriptor.Dispose();
            };

            _logDelegatePtr = Marshal.GetFunctionPointerForDelegate(logAction);
            _handle = GCHandle.Alloc(logAction);
        }

        public Plugin Load(string assemblyPath)
        {
            var assemblyTag = Path.GetFileName(Path.GetDirectoryName(assemblyPath)) + "|" + Path.GetFileName(assemblyPath);
            
            var lib = NativeLibrary.Load(assemblyPath);

            _logger.LogInformation(LogsDictionary.PluginLoader.LoadingStarted, assemblyTag);

            var result = TryMain(lib);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.Main);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.Main);
                    break;
                case 3:
                    throw new InvalidOperationException(
                        string.Format(LogsDictionary.PluginLoader.InitInternalError, assemblyPath));
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.EntryPointProcessed, assemblyTag);

            result = TryDelegateLogging(lib, _logDelegatePtr);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.DelegateLogging);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.DelegateLogging);
                    break;
                case 3:
                    throw new InvalidOperationException(
                        string.Format(LogsDictionary.PluginLoader.LoggingDelegateInternalError, assemblyPath));
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.LoggingDelegated, assemblyTag);
            
            result = TryBuild(lib);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.Build);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.Build);
                    break;
                case 3:
                    throw new InvalidOperationException(
                        string.Format(LogsDictionary.PluginLoader.BuildInternalError, assemblyPath));
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.PluginBuilt, assemblyTag);

            result = TryGetHandlers(lib, out var handlers);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.GetHandlers);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.GetHandlers);
                    break;
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.HandlersReceived, assemblyTag);

            result = TryGetInfo(lib, out var info);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.GetInfo);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.GetInfo);
                    break;
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.InfoReceived, assemblyTag);

            result = TryGetDisposeFunc(lib, assemblyPath, out var disposeFunc);

            switch (result)
            {
                case 1:
                    ThrowDoesntExport(WebOsCoreContext.Exports.Dispose);
                    break;
                case 2:
                    ThrowInvalidExport(WebOsCoreContext.Exports.Dispose);
                    break;
            }

            _logger.LogDebug(LogsDictionary.PluginLoader.DisposeFuncReceived, assemblyTag);

            _logger.LogInformation(LogsDictionary.PluginLoader.PluginLoaded, assemblyTag);

            return new()
            {
                Handlers = handlers,
                Info = info,
                DisposeFunc = disposeFunc,
                AssemblyPath = assemblyPath
            };
        }

        private static void ThrowDoesntExport(string export)
            => throw new InvalidOperationException(
                string.Format(LogsDictionary.PluginLoader.DoesntExport, export));

        private static void ThrowInvalidExport(string export)
            => throw new InvalidOperationException(
                string.Format(LogsDictionary.PluginLoader.InvalidExport, export));


        private static int TryMain(nint lib)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Main, out var export))
                return 1;

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                return 2;
            }

            var result = func();

            if (result != 0)
                return 3;

            return 0;
        }

        private static int TryDelegateLogging(nint lib, nint logDelegate)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.DelegateLogging, out var export))
                return 1;

            delegate* unmanaged<nint, int> func;

            try
            {
                func = (delegate* unmanaged<nint, int>)export;
            }
            catch
            {
                return 2;
            }

            var result = func(logDelegate);

            if (result != 0)
                return 3;

            return 0;
        }

        private static int TryBuild(nint lib)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Build, out var export))
                return 1;

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                return 2;
            }

            var result = func();

            if (result != 0)
                return 3;

            return 0;
        }

        private static int TryGetHandlers(nint lib, out EndpointHandler[] handlers)
        {
            handlers = [];

            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.GetHandlers, out var export))
                return 1;

            delegate* unmanaged<int*, EndpointHandlerDescriptor*> func;

            try
            {
                func = (delegate* unmanaged<int*, EndpointHandlerDescriptor*>)export;
            }
            catch
            {
                return 2;
            }

            int count = 0;

            var mem = func(&count);

            for (int i = 0; i < count; i++)
                handlers = [.. handlers.Append(EndpointHandler.FromDescriptor(ref mem[i]))];

            return 0;
        }

        private static int TryGetInfo(nint lib, out PluginInfo info)
        {
            info = new();

            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.GetInfo, out var export))
                return 1;

            delegate* unmanaged<PluginInfoDescriptor> func;

            try
            {
                func = (delegate* unmanaged<PluginInfoDescriptor>)export;
            }
            catch
            {
                return 2;
            }

            var descriptor = func();

            info = PluginInfo.FromDescriptor(descriptor);

            return 0;
        }

        private static int TryGetDisposeFunc(nint lib, string assemblyPath, out Action disposeFunc)
        {
            disposeFunc = () => { };

            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Dispose, out var export))
                return 1;

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                return 2;
            }

            disposeFunc = () =>
            {
                var result = func();

                if (result != 0)
                    throw new InvalidOperationException(
                        string.Format(LogsDictionary.PluginLoader.DisposeInternalError, assemblyPath));
            };

            return 0;
        }
    }
}
