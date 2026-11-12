using Sudzekai.LinCtrl.Abi.Managed;

using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions.Internals.Services.DataResolving
{
    public interface IHttpResponseDataResolver
    {
        ResponseContentType ContentType { get; }

        HttpResponseDto Resolve<T>(HttpResponseDto response, T data);
    }
}
