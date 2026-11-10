namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginAssembliesDisposer(AssemblyLifetimeHost host) : IPluginDisposer
    {
        public void Dispose(PluginInfo info)
        {
            List<Exception> exceptions = [];
            foreach (var assembly in info.LoadedAssemblies)
            {
                try
                {
                    host.Dispose(assembly);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException("Failed to release plugin assemblies", exceptions);
        }
    }
}
