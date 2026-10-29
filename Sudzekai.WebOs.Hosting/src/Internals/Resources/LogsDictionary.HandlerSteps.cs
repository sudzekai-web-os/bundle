
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
        public static class HandlerAuthorizationStepHandler
        {
            public static string AuthorizationSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Авторизация не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] ({step}) Authorization is not required; skipping this step",
                };

            public static string AuthorizationAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Авторизация добавлена. Роли: {roles}",
                    _ => "[{assemblyTag}] ({step}) Authorization added. Roles: {roles}",
                };
        }

        public static class HandlerLimitingStepHandler
        {
            public static string RateLimitingAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Добавлено ограничение частоты запросов. Политика: {policy}",
                    _ => "[{assemblyTag}] ({step}) Rate limiting added. Policy: {policy}",
                };
        }

        public static class HandlerOpenApiStepHandler
        {
            public static string OpenApiSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Конфигурация OpenAPI не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] ({step}) OpenAPI configuration is not required; skipping this step",
                };

            public static string OpenApiAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Конфигурация OpenAPI добавлена. Параметры: {openApiInfo}",
                    _ => "[{assemblyTag}] ({step}) OpenAPI configuration added. Parameters: {openApiInfo}",
                };
        }

        public static class HandlerRoutingStepHandler
        {
            public static string RoutingSkipped
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Конфигурация маршрутизации не требуется, шаг пропущен",
                    _ => "[{assemblyTag}] ({step}) Routing configuration is not required; skipping this step",
                };

            public static string RoutingAdded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] ({step}) Конфигурация маршрутизации добавлена. Параметры: {routingInfo}",
                    _ => "[{assemblyTag}] ({step}) Routing configuration added. Parameters: {routingInfo}",
                };
        }
    }
}