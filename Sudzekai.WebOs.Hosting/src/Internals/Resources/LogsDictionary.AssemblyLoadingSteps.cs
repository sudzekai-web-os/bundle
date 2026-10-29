
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
        public static class AssemblyMainStepHandler
        {
            public static string EntryPointProcessed
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Точка входа успешно обработана",
                    _ => "[{assemblyTag}] ({step}) Entry point processed successfully",
                };

            public static string InitInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка инициализации",
                    _ => "Internal error while initializing",
                };
        }

        public static class AssemblyDelegateLoggingStepHandler
        {
            public static string LoggingDelegated
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Логирование успешно делегировано",
                    _ => "[{assemblyTag}] ({step}) Logging delegated successfully",
                };

            public static string LoggingDelegateInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка делегирования логирования",
                    _ => "Internal error while delegating logging",
                };
        }

        public static class AssemblyBuildStepHandler
        {
            public static string PluginBuilt
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Успешно собрано",
                    _ => "[{assemblyTag}] ({step}) Built successfully",
                };

            public static string BuildInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка сборки",
                    _ => "Internal error while building",
                };
        }

        public static class AssemblyGetHandlersStepHandler
        {
            public static string HandlersReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Получены обработчики",
                    _ => "[{assemblyTag}] ({step}) Handlers received",
                };
        }

        public static class AssemblyGetHandlersMetadataStepHandler
        {
            public static string HandlersMetadataReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Получены OpenAPI-метаданные обработчиков",
                    _ => "[{assemblyTag}] ({step}) Handlers metadata received",
                };
        }

        public static class AssemblyGetInformationStepHandler
        {
            public static string InfoReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Информация получена",
                    _ => "[{assemblyTag}] ({step}) Information received",
                };
        }

        public static class AssemblyGetDisposeDelegateStepHandler
        {
            public static string DisposeFuncReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Получен делегат функции освобождения",
                    _ => "[{assemblyTag}] ({step}) Dispose delegate received",
                };

            public static string DisposeInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка: dispose вернул ненулевой код",
                    _ => "Internal error: dispose returned a non-zero code",
                };
        }
    }
}