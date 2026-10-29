using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    public static class ContentTypeExtensions
    {
        extension (ContentType contentType)
        {
            public string ToMediaTypeString()
                => contentType switch
                {
                    ContentType.ApplicationJson => "application/json",
                    ContentType.ApplicationXml => "application/xml",
                    ContentType.TextPlain => "text/plain",
                    ContentType.TextHtml => "text/html",
                    ContentType.TextCsv => "text/csv",
                    ContentType.ApplicationOctetStream => "application/octet-stream",
                    ContentType.ApplicationFormUrlEncoded => "application/x-www-form-urlencoded",
                    ContentType.MultipartFormData => "multipart/form-data",
                    ContentType.ImagePng => "image/png",
                    ContentType.ImageJpeg => "image/jpeg",
                    ContentType.ImageWebp => "image/webp",
                    ContentType.ImageSvgXml => "image/svg+xml",
                    ContentType.ApplicationPdf => "application/pdf",
                    ContentType.ApplicationZip => "application/zip",
                    _ => "text/plain"
                };
        }
    }
}
