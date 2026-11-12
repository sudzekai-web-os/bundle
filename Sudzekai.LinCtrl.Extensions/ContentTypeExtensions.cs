using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions
{
    public static class ContentTypeExtensions
    {
        extension(ResponseContentType contentType)
        {
            public static ResponseContentType FromMimeString(string? contentTypeString)
            {
                if (string.IsNullOrWhiteSpace(contentTypeString))
                    return ResponseContentType.None;

                var mediaType = contentTypeString.Split(';', 2)[0].Trim();

                return mediaType.ToLowerInvariant() switch
                {
                    "application/json" => ResponseContentType.ApplicationJson,
                    "text/plain" => ResponseContentType.TextPlain,
                    "text/html" => ResponseContentType.TextHtml,
                    "text/csv" => ResponseContentType.TextCsv,
                    "application/octet-stream" => ResponseContentType.ApplicationOctetStream,
                    "application/x-www-form-urlencoded" => ResponseContentType.ApplicationFormUrlEncoded,
                    "image/png" => ResponseContentType.ImagePng,
                    "image/jpeg" => ResponseContentType.ImageJpeg,
                    "image/webp" => ResponseContentType.ImageWebp,
                    "image/svg+xml" => ResponseContentType.ImageSvgXml,
                    "application/pdf" => ResponseContentType.ApplicationPdf,
                    "application/zip" => ResponseContentType.ApplicationZip,
                    _ => throw new ArgumentException($"Unsupported content type: '{contentTypeString}'", nameof(contentTypeString))
                };
            }

            public string ToMimeString() => contentType switch
            {
                ResponseContentType.ApplicationJson => "application/json",
                ResponseContentType.TextPlain => "text/plain",
                ResponseContentType.TextHtml => "text/html",
                ResponseContentType.TextCsv => "text/csv",
                ResponseContentType.ApplicationOctetStream => "application/octet-stream",
                ResponseContentType.ApplicationFormUrlEncoded => "application/x-www-form-urlencoded",
                ResponseContentType.ImagePng => "image/png",
                ResponseContentType.ImageJpeg => "image/jpeg",
                ResponseContentType.ImageWebp => "image/webp",
                ResponseContentType.ImageSvgXml => "image/svg+xml",
                ResponseContentType.ApplicationPdf => "application/pdf",
                ResponseContentType.ApplicationZip => "application/zip",
                _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null)
            };
        }
    }
}
