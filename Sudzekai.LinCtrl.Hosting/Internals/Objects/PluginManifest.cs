namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginManifest
    {
        public string Name { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string[] Assemblies { get; set; } = [];
    }
}
