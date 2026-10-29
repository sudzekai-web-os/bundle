
namespace Sudzekai.WebOs.Hosting
{
    internal static partial class LogsDictionary
    {
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

            public static string ManifestIsInvalid
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "manifest.json в директории {pluginDirPath} отсутствует, пуст или содержит невалидный JSON. Загрузка плагинов из этой директории пропущена...",
                    _ => "manifest.json in directory {pluginDirPath} doesn't exists, empty or contains invalid JSON. Plugin loading from this directory was skipped...",
                };

            public static string ManifestLoaded
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Манифест загружен. Сборки: {assemblies}",
                    _ => "[{pluginDirTag}] Manifest loaded. Assemblies: {assemblies}",
                };

            public static string AssembliesLoadingStarted
                => HostingContext.LogLanguage switch
                {
                    LogLanguage.Russian => "[{pluginDirTag}] Загрузка сборок плагина...",
                    _ => "[{pluginDirTag}] Loading plugin's assemblies...",
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
    }
}