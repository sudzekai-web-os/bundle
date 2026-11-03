
namespace Sudzekai.LinCtrl.Hosting 
{
    internal interface IPluginManifestFieldValidator
    {
        void ThrowIfInvalid(PluginManifest manifest);
    }
}
