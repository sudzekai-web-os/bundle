using Sudzekai.WebOs.Hosting.src.Internals.Context;

namespace Sudzekai.WebOs.Hosting
{
    internal class LogsDictionary
    {
        public static class HandlerConfigurator
        {
            public static string AuthorizationSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Авторизация не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] Authorization is not required; skipping this step",
                };

            public static string AuthorizationAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Авторизация добавлена. Роли: {roles}",
                    _ => "[{assemblyTag}] Authorization added. Roles: {roles}",
                };

            public static string RateLimitingAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Добавлено ограничение частоты запросов. Политика: {policy}",
                    _ => "[{assemblyTag}] Rate limiting added. Policy: {policy}",
                };

            public static string OpenApiSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Конфигурация OpenAPI не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] OpenAPI configuration is not required; skipping this step",
                };

            public static string OpenApiAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Конфигурация OpenAPI добавлена. Параметры: {openApiInfo}",
                    _ => "[{assemblyTag}] OpenAPI configuration added. Parameters: {openApiInfo}",
                };

            public static string RoutingSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Конфигурация маршрутизации не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] Routing configuration is not required; skipping this step",
                };

            public static string RoutingAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Конфигурация маршрутизации добавлена. Параметры: {routingInfo}",
                    _ => "[{assemblyTag}] Routing configuration added. Parameters: {routingInfo}",
                };

        }

        public static class PluginHandlersRegisterer
        {
            public static string PluginHandlersRegistrationStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Регистрация обработчиков плагина...",
                    _ => "[{assemblyTag}] Registering plugin handlers...",
                };

            public static string PluginHandlersRegistrationEnded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Зарегистрировано обработчиков плагина: {count}",
                    _ => "[{assemblyTag}] Plugin handlers registered: {count}",
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

        public static class PluginHost
        {
            public static string PluginsSearchStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Поиск плагинов...",
                    _ => "Searching for plugins...",
                };

            public static string PluginsDirCreated
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Создана директория \"plugins\"",
                    _ => "Created directory \"plugins\"",
                };

            public static string DirJsonFiles
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] JSON-файлы в директории: {files}",
                    _ => "[{pluginDirTag}] JSON files in directory: {files}",
                };

            public static string ManifestFound
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Найден манифест: {manifestFileName}",
                    _ => "[{pluginDirTag}] Manifest found: {manifestFileName}",
                };

            public static string PluginDirInitialized
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Директория плагинов инициализирована",
                    _ => "[{pluginDirTag}] Plugin directory initialized",
                };

            public static string ManifestIsInvalid
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "manifest.json в директории {pluginDirPath} содержит невалидный JSON. Загрузка плагинов из этой директории пропущена...",
                    _ => "manifest.json in directory {pluginDirPath} contains invalid JSON. Plugin loading from this directory was skipped...",
                };

            public static string ManifestLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Манифест загружен. Сборки: {assemblies}",
                    _ => "[{pluginDirTag}] Manifest loaded. Assemblies: {assemblies}",
                };

            public static string AssemblyException
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}|{assemblyFileName}] Загрузка плагина пропущена: {ex}",
                    _ => "[{pluginDirTag}|{assemblyFileName}] Plugin loading skipped: {ex}",
                };

            public static string AssembliesLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Загружено сборок: {count}",
                    _ => "[{pluginDirTag}] Assemblies loaded: {count}",
                };

            public static string PluginsLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Загружено плагинов: {count}",
                    _ => "Plugins loaded: {count}",
                };
        }

        public static class PluginLoader
        {
            public static string LoadingStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Загрузка плагина начата",
                    _ => "[{assemblyTag}] Plugin loading started",
                };

            public static string EntryPointProcessed
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (1/6) Точка входа успешно обработана",
                    _ => "[{assemblyTag}] (1/6) Entry point processed successfully",
                };

            public static string LoggingDelegated
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (2/6) Логирование успешно делегировано",
                    _ => "[{assemblyTag}] (2/6) Logging delegated successfully",
                };

            public static string PluginBuilt
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (3/6) Плагин успешно собран",
                    _ => "[{assemblyTag}] (3/6) Plugin built successfully",
                };

            public static string HandlersReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (4/6) Получены обработчики плагина",
                    _ => "[{assemblyTag}] (4/6) Plugin handlers received",
                };

            public static string InfoReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (5/6) Получена информация о плагине",
                    _ => "[{assemblyTag}] (5/6) Plugin information received",
                };

            public static string DisposeFuncReceived
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] (6/6) Получен делегат функции освобождения плагина",
                    _ => "[{assemblyTag}] (6/6) Plugin dispose delegate received",
                };

            public static string PluginLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Плагин успешно загружен",
                    _ => "[{assemblyTag}] Plugin loaded successfully",
                };

            public static string InitInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка инициализации плагина: {0}",
                    _ => "Internal error while initializing plugin: {0}",
                };

            public static string LoggingDelegateInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка делегирования логирования плагина: {0}",
                    _ => "Internal error while delegating plugin logging: {0}",
                };

            public static string BuildInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка сборки плагина: {0}",
                    _ => "Internal error while building plugin: {0}",
                };

            public static string DoesntExport
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Плагин не экспортирует \"{0}\"",
                    _ => "Plugin does not export \"{0}\"",
                };

            public static string InvalidExport
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Экспорт \"{0}\" плагина не соответствует контракту",
                    _ => "Plugin export \"{0}\" does not match the contract",
                };

            public static string DisposeInternalError
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Внутренняя ошибка плагина {0}: dispose вернул ненулевой код",
                    _ => "Internal plugin error for {0}: dispose returned a non-zero code",
                };
        }

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
