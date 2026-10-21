using System.Net;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Core
{
    public class HandlerResult
    {
        private HandlerResult(int statusCode)
        {
            StatusCode = statusCode;
            ContentType = ContentType.TextPlain;
        }


        public int StatusCode { get; private set; }

        public ContentType ContentType { get; private set; }

        public string? Data { get; private set; }

        public JsonTypeInfo? JsonTypeInfo { get; private set; }


        public static HandlerResult Create(HttpStatusCode statusCode)
            => new((int)statusCode);

        public HandlerResult WithData(string? data)
        {
            Data = data;
            return this;
        }

        public HandlerResult As(ContentType contentType)
        {
            ContentType = contentType;
            return this;
        }

        public HandlerResult WithTypeInfo(JsonTypeInfo typeInfo)
        {
            JsonTypeInfo = typeInfo;
            return this;
        }
    }
}
