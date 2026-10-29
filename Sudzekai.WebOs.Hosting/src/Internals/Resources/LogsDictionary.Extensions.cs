
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
        public static class EndpointHandlerBuilderExtensions
        {
            public static string InvalidLimitingPolicy
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Дескриптор содержит невалидную политику ограничения",
                    _ => "Descriptor contains an invalid rate limiting policy",
                };
        }

        public static class HandlerRequestBuilderExtensions
        {
            public static string InvalidHttpMethod
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Получен неподдерживаемый HTTP-метод",
                    _ => "Unsupported HTTP method received",
                };
        }

        public static class HandlerResultDescriptorExtensions
        {
            public static string InvalidStatusCode
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Дескриптор содержит невалидный HTTP-статус",
                    _ => "Descriptor contains an invalid HTTP status code",
                };

            public static string UnknownContentType
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Неизвестный тип содержимого дескриптора: {0}",
                    _ => "Unknown descriptor content type: {0}",
                };
        }
    }
}