using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginAssembliesLoader(
        AssemblyLifetimeHost assemblyHost,
        ILogger<PluginAssembliesLoader> logger) : IPluginLoader
    {
        public void Load(PluginInfo info)
        {
            if (info.Manifest == null)
                throw new InvalidOperationException("Plugin manifest was null");

            if (info.Manifest.Assemblies.Length == 0)
                return;

            List<AssemblyInfo> assemblies = [];

            foreach (var assemblyPath in info.Manifest.Assemblies)
            {
                var assemblyFullPath = Path.Combine(info.DirectoryPath, assemblyPath);

                try
                {
                    var assembly = assemblyHost.Load(assemblyFullPath);
                    assembly.PluginInfo = info;
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    var tag = Path.GetFileName(Path.GetDirectoryName(assemblyFullPath)) + "/" + Path.GetFileNameWithoutExtension(assemblyFullPath);

                    logger.LogError("[{tag}] Exception while loading assembly: {message}. Assembly skipped", tag, ex.Message);
                }
            }

            info.Assemblies = assemblies;
        }
    }
}
