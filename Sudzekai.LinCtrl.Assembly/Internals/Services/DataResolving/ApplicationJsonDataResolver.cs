using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ApplicationJsonDataResolver : IHttpResponseDataResolver
    {
        public ResponseContentType ContentType => ResponseContentType.ApplicationJson;

        public HttpResponseDto Resolve<T>(HttpResponseDto response, T data)
        {
            if (data is null)
            {
                response.Data = [];
                return response;
            }

            Type runtimeType = data.GetType();
            JsonTypeInfo typeInfo = LinCtrlContext.JsonSerializerOptions.GetTypeInfo(runtimeType);

            response.Data = JsonSerializer.SerializeToUtf8Bytes(data, typeInfo);
            return response;
        }
    }
}