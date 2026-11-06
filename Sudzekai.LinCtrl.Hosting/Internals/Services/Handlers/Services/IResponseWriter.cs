using Sudzekai.LinCtrl.Core;
using AspHttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace Sudzekai.LinCtrl.Hosting
{
    internal interface IResponseWriter
    {
        ContentType ContentType { get; }
        Task WriteAsync(AspHttpResponse ctx, byte[] data);
    }
}
