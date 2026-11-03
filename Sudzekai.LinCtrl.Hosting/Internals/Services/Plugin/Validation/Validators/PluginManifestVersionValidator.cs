using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginManifestVersionValidator : IPluginManifestFieldValidator
    {
        private readonly ILogger<IPluginManifestFieldValidator> _logger;

        public PluginManifestVersionValidator(ILogger<IPluginManifestFieldValidator> logger)
        {
            _logger = logger;
        }

        public void ThrowIfInvalid(PluginManifest manifest)
        {
            if (manifest.Version.Split(".").Length != 3)
                throw new InvalidOperationException($"Plugin manifest \"Version\" field is invalid. (Version: {manifest.Version})");

            _logger.LogInformation("Plugin manifest Version field is valid ({Version})", manifest.Version);
        }
    }
}
