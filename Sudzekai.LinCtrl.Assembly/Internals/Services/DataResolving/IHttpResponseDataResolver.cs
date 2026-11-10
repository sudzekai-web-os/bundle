using Sudzekai.LinCtrl.Abi.Managed;

using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    public interface IHttpResponseDataResolver
    {
        ResponseContentType ContentType { get; }

        HttpResponseDto Resolve<T>(HttpResponseDto response, T data);
    }
}
