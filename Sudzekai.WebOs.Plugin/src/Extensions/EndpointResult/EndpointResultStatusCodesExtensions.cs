using Sudzekai.WebOs.Core;
using System.Net;

namespace Sudzekai.WebOs.Plugin
{
    public static class EndpointResultStatusCodesExtensions
    {
        extension (HandlerResult)
        {
            public static HandlerResult Ok()
                => HandlerResult.Create(HttpStatusCode.OK);

            public static HandlerResult NoContent()
                => HandlerResult.Create(HttpStatusCode.NoContent);

            public static HandlerResult Forbidden()
                => HandlerResult.Create(HttpStatusCode.Forbidden);

            public static HandlerResult BadRequest()
                => HandlerResult.Create(HttpStatusCode.BadRequest);

            public static HandlerResult InternalServerError()
                => HandlerResult.Create(HttpStatusCode.InternalServerError);

            public static HandlerResult NotFound()
                => HandlerResult.Create(HttpStatusCode.NotFound);
        }
    }
}
