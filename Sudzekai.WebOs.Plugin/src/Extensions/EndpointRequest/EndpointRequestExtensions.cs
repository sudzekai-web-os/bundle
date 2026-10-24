using Sudzekai.WebOs.Core;
using System.Collections.Specialized;
using System.Web;

namespace Sudzekai.WebOs.Plugin
{
    public static class EndpointRequestExtensions
    {
        public static NameValueCollection GetQuery(this HandlerRequest request)
            => HttpUtility.ParseQueryString(request.QueryString);
    }
}
