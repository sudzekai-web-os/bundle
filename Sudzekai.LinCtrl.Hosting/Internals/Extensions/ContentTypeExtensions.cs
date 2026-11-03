using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal static class ContentTypeExtensions
    {
        extension(ContentType contentType)
        {
            public static ContentType FromMimeString(string? contentTypeString)
            {
                if (string.IsNullOrWhiteSpace(contentTypeString))
                    return ContentType.None;

                var mediaType = contentTypeString.Split(';', 2)[0].Trim();

                return mediaType.ToLowerInvariant() switch
                {
                    "application/json" => ContentType.ApplicationJson,
                    "text/plain" => ContentType.TextPlain,
                    "text/html" => ContentType.TextHtml,
                    "text/csv" => ContentType.TextCsv,
                    "application/octet-stream" => ContentType.ApplicationOctetStream,
                    "application/x-www-form-urlencoded" => ContentType.ApplicationFormUrlEncoded,
                    "image/png" => ContentType.ImagePng,
                    "image/jpeg" => ContentType.ImageJpeg,
                    "image/webp" => ContentType.ImageWebp,
                    "image/svg+xml" => ContentType.ImageSvgXml,
                    "application/pdf" => ContentType.ApplicationPdf,
                    "application/zip" => ContentType.ApplicationZip,
                    _ => throw new ArgumentException($"Unsupported content type: '{contentTypeString}'", nameof(contentTypeString))
                };
            }

            public string ToMimeString() => contentType switch
            {
                ContentType.ApplicationJson => "application/json",
                ContentType.TextPlain => "text/plain",
                ContentType.TextHtml => "text/html",
                ContentType.TextCsv => "text/csv",
                ContentType.ApplicationOctetStream => "application/octet-stream",
                ContentType.ApplicationFormUrlEncoded => "application/x-www-form-urlencoded",
                ContentType.ImagePng => "image/png",
                ContentType.ImageJpeg => "image/jpeg",
                ContentType.ImageWebp => "image/webp",
                ContentType.ImageSvgXml => "image/svg+xml",
                ContentType.ApplicationPdf => "application/pdf",
                ContentType.ApplicationZip => "application/zip",
                _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null)
            };
        }
    }
}
