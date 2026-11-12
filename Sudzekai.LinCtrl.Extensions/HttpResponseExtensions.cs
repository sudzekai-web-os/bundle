using Sudzekai.LinCtrl.Extensions.Internals.Services.DataResolving;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Net;
using System.Buffers;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Assembly.Extensions
{
    public static class HttpResponseExtensions
    {
        extension (HttpResponseDto response)
        {
            public static HttpResponseDto Create(HttpStatusCode statusCode)
                => new() { StatusCode = statusCode };

            public static HttpResponseDto CreateError(HttpStatusCode statusCode, int code, string message)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan((int)statusCode, 400);
                ArgumentOutOfRangeException.ThrowIfGreaterThan((int)statusCode, 599);
                ArgumentNullException.ThrowIfNull(message);

                var buffer = new ArrayBufferWriter<byte>();
                using var writer = new Utf8JsonWriter(buffer);
                writer.WriteStartObject();
                writer.WriteNumber("code", code);
                writer.WriteString("message", message);
                writer.WriteEndObject();
                writer.Flush();
                return new() { StatusCode = statusCode, ContentType = ResponseContentType.ApplicationJson,
                    Data = buffer.WrittenSpan.ToArray() };
            }

            public HttpResponseDto As(ResponseContentType contentType)
            {
                response.ContentType = contentType;
                return response;
            }

            public HttpResponseDto WithData<T>(T data)
            {
                IHttpResponseDataResolver resolver = response.ContentType switch
                {
                    ResponseContentType.ApplicationJson => new ApplicationJsonDataResolver(),
                    ResponseContentType.TextPlain => new TextPlainDataResolver(),
                    ResponseContentType.TextHtml => new TextHtmlDataResolver(),
                    ResponseContentType.TextCsv => new TextCsvDataResolver(),
                    ResponseContentType.ApplicationOctetStream => new ApplicationOctetStreamDataResolver(),
                    ResponseContentType.ApplicationFormUrlEncoded => new ApplicationFormUrlEncodedDataResolver(),
                    ResponseContentType.ImagePng => new ImagePngDataResolver(),
                    ResponseContentType.ImageJpeg => new ImageJpegDataResolver(),
                    ResponseContentType.ImageWebp => new ImageWebpDataResolver(),
                    ResponseContentType.ImageSvgXml => new ImageSvgXmlDataResolver(),
                    ResponseContentType.ApplicationPdf => new ApplicationPdfDataResolver(),
                    ResponseContentType.ApplicationZip => new ApplicationZipDataResolver(),
                    _ => throw new NotSupportedException($"Unsupported response content type: {response.ContentType}")
                };
                return resolver.Resolve(response, data);
            }

            // ApplicationJson 

            public static HttpResponseDto CreateJson(HttpStatusCode statusCode, object data)
                => HttpResponseDto.Create(statusCode).AsJson(data);
            
            public HttpResponseDto AsJson(object data)
                => response.As(ResponseContentType.ApplicationJson).WithData(data);

            // TextPlain 
            
            public static HttpResponseDto CreateText(HttpStatusCode statusCode, string data)
                => HttpResponseDto.Create(statusCode).AsText(data);
            
            public HttpResponseDto AsText(string data)
                => response.As(ResponseContentType.TextPlain).WithData(data);

            // TextHtml 
            
            public static HttpResponseDto CreateHtml(HttpStatusCode statusCode, string data)
                => HttpResponseDto.Create(statusCode).AsHtml(data);

            public HttpResponseDto AsHtml(string data)
                => response.As(ResponseContentType.TextHtml).WithData(data);

            // TextCsv 
            
            public static HttpResponseDto CreateCsv(HttpStatusCode statusCode, string data)
                => HttpResponseDto.Create(statusCode).AsCsv(data);
            
            public HttpResponseDto AsCsv(string data)
                => response.As(ResponseContentType.TextCsv).WithData(data);

            // ApplicationOctetStream 
            
            public static HttpResponseDto CreateOctetStream(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsOctetStream(data);
            
            public HttpResponseDto AsOctetStream(byte[] data)
                => response.As(ResponseContentType.ApplicationOctetStream).WithData(data);

            // ApplicationFormUrlEncoded 
            
            public static HttpResponseDto CreateFormUrlEncoded(HttpStatusCode statusCode, string data)
                => HttpResponseDto.Create(statusCode).AsFormUrlEncoded(data);
            
            public HttpResponseDto AsFormUrlEncoded(string data)
                => response.As(ResponseContentType.ApplicationFormUrlEncoded).WithData(data);

            // ImagePng 
            
            public static HttpResponseDto CreatePng(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsPng(data);
            
            public HttpResponseDto AsPng(byte[] data)
                => response.As(ResponseContentType.ImagePng).WithData(data);

            // ImageJpeg 
            
            public static HttpResponseDto CreateJpeg(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsJpeg(data);
            
            public HttpResponseDto AsJpeg(byte[] data)
                => response.As(ResponseContentType.ImageJpeg).WithData(data);

            // ImageWebp 
            
            public static HttpResponseDto CreateWebp(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsWebp(data);
           
            public HttpResponseDto AsWebp(byte[] data)
                => response.As(ResponseContentType.ImageWebp).WithData(data);

            // ImageSvgXml 
            
            public static HttpResponseDto CreateSvg(HttpStatusCode statusCode, string data)
                => HttpResponseDto.Create(statusCode).AsSvg(data);
            
            public HttpResponseDto AsSvg(string data)
                => response.As(ResponseContentType.ImageSvgXml).WithData(data);

            // ApplicationPdf 
            
            public static HttpResponseDto CreatePdf(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsPdf(data);
            
            public HttpResponseDto AsPdf(byte[] data)
                => response.As(ResponseContentType.ApplicationPdf).WithData(data);

            // ApplicationZip 
            
            public static HttpResponseDto CreateZip(HttpStatusCode statusCode, byte[] data)
                => HttpResponseDto.Create(statusCode).AsZip(data);
            
            public HttpResponseDto AsZip(byte[] data)
                => response.As(ResponseContentType.ApplicationZip).WithData(data);
        }
    }
}
