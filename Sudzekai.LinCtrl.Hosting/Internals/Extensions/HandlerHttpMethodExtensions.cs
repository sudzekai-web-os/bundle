using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class HandlerHttpMethodExtensions
    {
        extension(HttpRequestMethod method)
        {
            public string ToMethodString()
                => method.ToString().ToLower();

            public static HttpRequestMethod FromHttpMethod(HttpMethod met)
            {
                if (met == HttpMethod.Get)
                    return HttpRequestMethod.Get;
            
                if (met == HttpMethod.Post)
                    return HttpRequestMethod.Post;

                if (met == HttpMethod.Put)
                    return HttpRequestMethod.Put;

                if (met == HttpMethod.Patch)
                    return HttpRequestMethod.Patch;

                if (met == HttpMethod.Delete) 
                    return HttpRequestMethod.Delete;

                throw new ArgumentOutOfRangeException(nameof(met), met, "");
            }

            public static HttpRequestMethod FromHttpMethodString(string met)
                => met.ToLower() switch
                {
                    "get" => HttpRequestMethod.Get,
                    "post" => HttpRequestMethod.Post,
                    "delete" => HttpRequestMethod.Delete,
                    "put" => HttpRequestMethod.Put,
                    "patch" => HttpRequestMethod.Patch,
                    _ => throw new ArgumentOutOfRangeException(nameof(met), met, "")
                };
        }
    }
}
