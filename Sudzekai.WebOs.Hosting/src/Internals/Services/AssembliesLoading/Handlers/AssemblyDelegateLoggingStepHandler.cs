using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyDelegateLoggingStepHandler : IAssemblyLoadingStepHandler
    {
        private unsafe delegate void LogDelegate(LogEntryDescriptor* descriptor);
        private readonly GCHandle _handle;
        private readonly nint _logDelegatePtr;

        public AssemblyDelegateLoggingStepHandler(IServiceProvider provider)
        {
            var logDelegate = GetLogDelegate(provider);
            
            _handle = GCHandle.Alloc(logDelegate);

            _logDelegatePtr = Marshal.GetFunctionPointerForDelegate(logDelegate);
        }

        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.DelegateLogging, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.DelegateLogging));

            delegate* unmanaged<nint, int> func;

            try
            {
                func = (delegate* unmanaged<nint, int>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.DelegateLogging));
            }

            var result = func(_logDelegatePtr);

            if (result != 0)
                throw new InvalidOperationException(LogsDictionary.AssemblyDelegateLoggingStepHandler.LoggingDelegateInternalError);

            return LogsDictionary.AssemblyDelegateLoggingStepHandler.LoggingDelegated;
        }

        private unsafe LogDelegate GetLogDelegate(IServiceProvider provider)
            => descriptor =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                var category = AllocationUtilities.ToString(descriptor->Category, descriptor->CategoryLength);

                var logger = loggerFactory.CreateLogger(category);

                var message = AllocationUtilities.ToString(descriptor->Message, descriptor->MessageLength);

                logger.Log((LogLevel)descriptor->LogLevel, message);

                descriptor->Dispose();
            };
    }
}
