namespace Sudzekai.WebOs.Hosting
{
    internal interface IAssemblyLoadingStepHandler
    {
        unsafe string Handle(nint lib, PluginAssembly plugin);
    }
}
