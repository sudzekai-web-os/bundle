using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ApplicationJsonDataResolver : IHttpResponseDataResolver
    {
        public ContentType ContentType => ContentType.ApplicationJson;

        public HttpResponse Resolve<T>(HttpResponse response, T data)
        {
            if (data is null)
            {
                response.Data = [];
                return response;
            }

            Type runtimeType = data.GetType();
            JsonTypeInfo typeInfo = CoreContext.JsonSerializerOptions.GetTypeInfo(runtimeType);

            response.Data = JsonSerializer.SerializeToUtf8Bytes(data, typeInfo);
            return response;
        }
    }
}