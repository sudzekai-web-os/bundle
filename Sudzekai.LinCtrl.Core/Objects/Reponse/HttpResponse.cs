using System.Net;

namespace Sudzekai.LinCtrl.Core 
{
    public class HttpResponse(HttpStatusCode statusCode)
    {
        #region Data resolving

        private static readonly IList<IHttpResponseDataResolver> _resolvers = [
            new ApplicationJsonDataResolver(),
            new TextPlainDataResolver(),
            new TextHtmlDataResolver(),
            new TextCsvDataResolver(),
            new ApplicationOctetStreamDataResolver(),
            new ApplicationFormUrlEncodedDataResolver(),
            new ImagePngDataResolver(),
            new ImageJpegDataResolver(),
            new ImageWebpDataResolver(),
            new ImageSvgXmlDataResolver(),
            new ApplicationPdfDataResolver(),
            new ApplicationZipDataResolver()
        ];

        public static void AddResponseDataResolver(IHttpResponseDataResolver resolver)
        {
            if (_resolvers.FirstOrDefault(r => r.ContentType == resolver.ContentType) is var existing and not null)
                _resolvers.Remove(existing);

            _resolvers.Add(resolver);
        }

        #endregion

        public HttpStatusCode StatusCode { get; internal set; } = statusCode;

        public byte[] Data { get; internal set; } = [];

        public ContentType ContentType { get; internal set; } = ContentType.TextPlain;

        public static HttpResponse Create(HttpStatusCode statusCode)
            => new(statusCode);

        public HttpResponse As(ContentType contentType)
        {
            ContentType = contentType;
            return this;
        }

        public HttpResponse WithData<T>(T data)
        {
            var resolver = _resolvers.FirstOrDefault(r => r.ContentType == ContentType)
                ?? throw new NotSupportedException($"Recieved content type is not supported (ContentType = {ContentType}");

            return resolver.Resolve(this, data);
        }
    }
}
