using Sudzekai.WebOs.Core;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Plugin
{
    public static class EndpointResultContentTypeExtensions
    {
        extension (HandlerResult result)
        {
            public HandlerResult AsJson(JsonTypeInfo jsonTypeInfo)
                => result.As(ContentType.ApplicationJson).WithTypeInfo(jsonTypeInfo);

            public HandlerResult AsTextPlain()
                => result.As(ContentType.TextPlain);

            public HandlerResult AsTextHtml()
                => result.As(ContentType.TextHtml);

            public HandlerResult AsTextCsv()
                => result.As(ContentType.TextCsv);

            public HandlerResult AsOctetStream()
                => result.As(ContentType.ApplicationOctetStream);

            public HandlerResult AsFormUrlEncoded()
                => result.As(ContentType.ApplicationFormUrlEncoded);

            public HandlerResult AsMultipartFormData()
                => result.As(ContentType.MultipartFormData);

            public HandlerResult AsImagePng()
                => result.As(ContentType.ImagePng);

            public HandlerResult AsImageJpeg()
                => result.As(ContentType.ImageJpeg);

            public HandlerResult AsImageWebp()
                => result.As(ContentType.ImageWebp);

            public HandlerResult AsImageSvgXaml()
                => result.As(ContentType.ImageSvgXml);

            public HandlerResult AsPdf()
                => result.As(ContentType.ApplicationPdf);

            public HandlerResult AsZip()
                => result.As(ContentType.ApplicationZip);
        }
    }
}
