using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;

namespace LinCtrlAssembly
{
    public static class EntryPoint
    {
        [UnmanagedCallersOnly(EntryPoint = AssemblyExports.GetVTable)]
        public static unsafe AssemblyVTableDescriptor* GetVTable()
        {
            AssemblyContext ctx = new()
            {
                InitializeDelegate = Init,
                DelegateLoggingDelegate = DelegateLogging,
                BuildDelegate = Build,
                GetHttpRequestHandlersDelegate = GetHandlers
            };

            return ctx.GetVtbl();
        }

        private static AssemblyHostBuilder _builder = default!;
        private static AssemblyHost _host = default!;

        private static int Init()
        {
            _builder = AssemblyHost.CreateBuilder();

            return 0;
        }

        private static int DelegateLogging(LogDelegate del)
        {
            return 0;
        }

        private static int Build()
        {
            _host = _builder.Build();

            return 0;
        }

        private static IHttpRequestHandler[] GetHandlers()
            => _host.GetHandlers();
    }
}
