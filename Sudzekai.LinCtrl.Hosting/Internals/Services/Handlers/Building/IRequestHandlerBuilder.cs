using Microsoft.AspNetCore.Routing;

namespace Sudzekai.LinCtrl.Hosting
{
    internal interface IRequestHandlerBuilder
    {
        void Build(HttpRequestHandlerBuilder builder);
    }
}
