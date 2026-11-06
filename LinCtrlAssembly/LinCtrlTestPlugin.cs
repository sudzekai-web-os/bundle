using LinCtrlAssembly.Context;
using LinCtrlAssembly.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;

namespace LinCtrlAssembly
{
    public class LinCtrlTestPlugin
    {
        private static AssemblyContext _ctx = default!;
        private static AssemblyHostBuilder _builder = default!;
        private static AssemblyHost _host = default!;


        [UnmanagedCallersOnly(EntryPoint = AssemblyExports.ExchangeVTables)]
        public static unsafe int GetVTable(HostVTableDescriptor* hostVTable, AssemblyVTableDescriptor** assemblyVTable)
        {
            try
            {
                CoreContext.AddTypeInfoResolver(AssemblyJsonContext.Default);

                _ctx = new(hostVTable)
                {
                    InitializeDelegate = Init,
                    DelegateLoggingDelegate = DelegateLogging,
                    BuildDelegate = Build,
                    GetHttpRequestHandlersDelegate = GetHandlers
                };

                *assemblyVTable = _ctx.GetVtbl();
                
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private static int Init()
        {
            _builder = AssemblyHost.CreateBuilder(_ctx);

            _builder.Services.AddScoped<IHttpRequestHandler, ResponseObjectRequestHandler>();
            _builder.Services.AddScoped<IHttpRequestHandler, ResponseStringRequestHandler>();
            _builder.Services.AddScoped<IHttpRequestHandler, UnregisteredResponseObjectHandler>();

            return 0;
        }

        private static int DelegateLogging(LogDelegate del)
            => _builder.DelegateLogging(del);

        private static int Build()
        {
            _host = _builder.Build();

            var logger = _host.Services.GetRequiredService<ILogger<LinCtrlTestPlugin>>();

            logger.LogTrace("Trace log test");
            logger.LogDebug("Debug log test");
            logger.LogInformation("Info log test");
            logger.LogWarning("Warning log test");
            logger.LogError("Error log test");
            logger.LogCritical("Critical log test");

            return 0;
        }

        private static IHttpRequestHandler[] GetHandlers()
            => _host.GetHandlers();
    }
}
