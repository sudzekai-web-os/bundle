
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
        public static class PluginHandlersRegisterer
        {
            public static string PluginHandlersRegistrationStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Регистрация обработчиков сборки...",
                    _ => "[{assemblyTag}] Registering assembly handlers...",
                };

            public static string PluginHandlersRegistrationEnded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Зарегистрировано обработчиков сборки: {count}",
                    _ => "[{assemblyTag}] Assembly handlers registered: {count}",
                };

            public static string HandlerRegistrationStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Регистрация обработчика {method} -> {pattern}...",
                    _ => "[{assemblyTag}] Registering handler {method} -> {pattern}...",
                };

            public static string HandlerRegistrationEnded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Обработчик {method} -> {pattern} зарегистрирован",
                    _ => "[{assemblyTag}] Handler {method} -> {pattern} registered",
                };

            public static string HandlerUnsupportedContentType
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{Method}] {Pattern}: фильтрация ответа пропущена — неподдерживаемый тип содержимого '{ContentType}' (поддерживается только 'application/json')",
                    _ => "Response filtering skipped for {Method} {Pattern}: unsupported content type '{ContentType}' (only 'application/json' is supported)",
                };
        }
    }
}