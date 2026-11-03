using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginManifestValidator(
        IEnumerable<IPluginManifestFieldValidator> validators,
        ILogger<PluginManifestValidator> logger)
    {
        public void ThrowIfInvalid(PluginManifest manifest)
        {
            foreach (var validator in validators)
                validator.ThrowIfInvalid(manifest);

            logger.LogInformation("Plugin manifest is valid");
        }
    }
}
