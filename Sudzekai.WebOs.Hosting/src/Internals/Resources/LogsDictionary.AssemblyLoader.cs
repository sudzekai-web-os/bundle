
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
        public static class AssemblyLoader
        {
            public static string LoadingStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Загрузка сборки начата",
                    _ => "[{assemblyTag}] Assembly loading started",
                };

            public static string AssemblyLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{assemblyTag}] Сборка успешно загружена",
                    _ => "[{assemblyTag}] Assembly loaded successfully",
                };
        }

        public static class AssemblyExport
        {
            public static string DoesntExport
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Сборка не экспортирует \"{0}\"",
                    _ => "Assembly does not export \"{0}\"",
                };

            public static string InvalidExport
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "Экспорт \"{0}\" сборки не соответствует контракту",
                    _ => "Assembly export \"{0}\" does not match the contract",
                };
        }
    }
}