using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginManifestAssembilesValidator : IPluginManifestFieldValidator
    {
        private readonly ILogger<IPluginManifestFieldValidator> _logger;

        public PluginManifestAssembilesValidator(ILogger<IPluginManifestFieldValidator> logger)
        {
            _logger = logger;
        }

        public void ThrowIfInvalid(PluginManifest manifest)
        {
            if (manifest.Assemblies.Length == 0)
                throw new InvalidOperationException($"Plugin manifest \"Assemblies\" field contains no paths. (Assemblies: {string.Join(", ", manifest.Assemblies)})");

            _logger.LogInformation("Plugin manifest Assemblies field is valid ({count} entries)", manifest.Assemblies.Length);
        }
    }
}
