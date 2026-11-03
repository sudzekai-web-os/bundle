using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal interface IRequestHandlerUnloadingStepHandler
    {
        public void Unload(IHttpRequestHandler handler);
    }
}
