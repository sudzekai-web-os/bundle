using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class HandlerHttpMethodExtensions
    {
        extension(RequestHandlerHttpMethod method)
        {
            public string ToMethodString()
                => method.ToString().ToLower();

            public static RequestHandlerHttpMethod FromHttpMethod(HttpMethod met)
            {
                if (met == HttpMethod.Get)
                    return RequestHandlerHttpMethod.Get;
            
                if (met == HttpMethod.Post)
                    return RequestHandlerHttpMethod.Post;

                if (met == HttpMethod.Put)
                    return RequestHandlerHttpMethod.Put;

                if (met == HttpMethod.Patch)
                    return RequestHandlerHttpMethod.Patch;

                if (met == HttpMethod.Delete) 
                    return RequestHandlerHttpMethod.Delete;

                throw new ArgumentOutOfRangeException(nameof(met), met, "");
            }

            public static RequestHandlerHttpMethod FromHttpMethodString(string met)
                => met.ToLower() switch
                {
                    "get" => RequestHandlerHttpMethod.Get,
                    "post" => RequestHandlerHttpMethod.Post,
                    "delete" => RequestHandlerHttpMethod.Delete,
                    "put" => RequestHandlerHttpMethod.Put,
                    "patch" => RequestHandlerHttpMethod.Patch,
                    _ => throw new ArgumentOutOfRangeException(nameof(met), met, "")
                };
        }
    }
}
