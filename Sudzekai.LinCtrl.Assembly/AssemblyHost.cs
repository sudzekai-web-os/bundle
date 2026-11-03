using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHost
    {
        internal AssemblyHost(IServiceProvider provider)
        {
            Services = provider;
        }

        public IServiceProvider Services;

        public static AssemblyHostBuilder CreateBuilder()
            => new();

        public unsafe AssemblyVTableDescriptor* GetVTable()
        {
            AssemblyVTable vTable = new(AssemblyContext.AbiVersion)
            {
                FreeAssemblyVTableDelegate = GetFreeVTableDelegate()
            };

            var pointer = AllocationUtilities.AllocCoTaskMem(AssemblyVTableDescriptor.Alloc(vTable));

            return pointer;
        }

        public IHttpRequestHandler[] GetHandlers()
        {
            var handlers = Services.GetServices<IHttpRequestHandler>();

            return [.. handlers];
        }

        private static unsafe FreeAssemblyVTableDelegate GetFreeVTableDelegate()
            => (vTableDescriptorPtr) =>
            {
                if (vTableDescriptorPtr == null)
                    return 1;

                try
                {

                    vTableDescriptorPtr->Free();
                    return 0;
                }
                catch
                {
                    return 2;
                }
            };
    }
}
