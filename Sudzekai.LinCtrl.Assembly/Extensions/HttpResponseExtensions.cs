using Sudzekai.LinCtrl.Core;
using System.Net;

namespace Sudzekai.LinCtrl.Assembly.Extensions
{
    public static class HttpResponseExtensions
    {
        extension (HttpResponse response)
        {
            // ApplicationJson 

            public static HttpResponse CreateJson(HttpStatusCode statusCode, object data)
                => HttpResponse.Create(statusCode).AsJson(data);
            
            public HttpResponse AsJson(object data)
                => response.As(ResponseContentType.ApplicationJson).WithData(data);

            // TextPlain 
            
            public static HttpResponse CreateText(HttpStatusCode statusCode, string data)
                => HttpResponse.Create(statusCode).AsText(data);
            
            public HttpResponse AsText(string data)
                => response.As(ResponseContentType.TextPlain).WithData(data);

            // TextHtml 
            
            public static HttpResponse CreateHtml(HttpStatusCode statusCode, string data)
                => HttpResponse.Create(statusCode).AsHtml(data);

            public HttpResponse AsHtml(string data)
                => response.As(ResponseContentType.TextHtml).WithData(data);

            // TextCsv 
            
            public static HttpResponse CreateCsv(HttpStatusCode statusCode, string data)
                => HttpResponse.Create(statusCode).AsCsv(data);
            
            public HttpResponse AsCsv(string data)
                => response.As(ResponseContentType.TextCsv).WithData(data);

            // ApplicationOctetStream 
            
            public static HttpResponse CreateOctetStream(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsOctetStream(data);
            
            public HttpResponse AsOctetStream(byte[] data)
                => response.As(ResponseContentType.ApplicationOctetStream).WithData(data);

            // ApplicationFormUrlEncoded 
            
            public static HttpResponse CreateFormUrlEncoded(HttpStatusCode statusCode, string data)
                => HttpResponse.Create(statusCode).AsFormUrlEncoded(data);
            
            public HttpResponse AsFormUrlEncoded(string data)
                => response.As(ResponseContentType.ApplicationFormUrlEncoded).WithData(data);

            // ImagePng 
            
            public static HttpResponse CreatePng(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsPng(data);
            
            public HttpResponse AsPng(byte[] data)
                => response.As(ResponseContentType.ImagePng).WithData(data);

            // ImageJpeg 
            
            public static HttpResponse CreateJpeg(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsJpeg(data);
            
            public HttpResponse AsJpeg(byte[] data)
                => response.As(ResponseContentType.ImageJpeg).WithData(data);

            // ImageWebp 
            
            public static HttpResponse CreateWebp(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsWebp(data);
           
            public HttpResponse AsWebp(byte[] data)
                => response.As(ResponseContentType.ImageWebp).WithData(data);

            // ImageSvgXml 
            
            public static HttpResponse CreateSvg(HttpStatusCode statusCode, string data)
                => HttpResponse.Create(statusCode).AsSvg(data);
            
            public HttpResponse AsSvg(string data)
                => response.As(ResponseContentType.ImageSvgXml).WithData(data);

            // ApplicationPdf 
            
            public static HttpResponse CreatePdf(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsPdf(data);
            
            public HttpResponse AsPdf(byte[] data)
                => response.As(ResponseContentType.ApplicationPdf).WithData(data);

            // ApplicationZip 
            
            public static HttpResponse CreateZip(HttpStatusCode statusCode, byte[] data)
                => HttpResponse.Create(statusCode).AsZip(data);
            
            public HttpResponse AsZip(byte[] data)
                => response.As(ResponseContentType.ApplicationZip).WithData(data);
        }
    }
}
