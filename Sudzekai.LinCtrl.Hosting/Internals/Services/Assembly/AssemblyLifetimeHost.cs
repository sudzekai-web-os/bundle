using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyLifetimeHost(
        IEnumerable<IAssemblyLoader> loaders,
        IEnumerable<IAssemblyInitializer> initializers,
        ILogger<AssemblyLifetimeHost> logger)
    {
        public AssemblyInfo Load(string fullPath)
        {
            AssemblyInfo assembly = new(fullPath);

            var tag = assembly.GetTag();

            logger.LogInformation("[{tag}] Assembly loading started", tag);

            if (!Path.IsPathFullyQualified(fullPath))
                throw new InvalidOperationException($"[{tag}] Assembly path must be fully qualified");

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"[{tag}] Assembly path not found");

            foreach (var loader in loaders)
                loader.Load(assembly);

            assembly.SetLoaded();

            logger.LogInformation("[{tag}] Assembly loaded", tag);

            return assembly;
        }

        public void Initialize(AssemblyInfo assembly)
        {
            var tag = assembly.GetTag();

            logger.LogInformation("[{tag}] Assembly initialization started", tag);

            if (assembly.Lifetime != AssemblyLifetime.Loaded)
            {
                if (assembly.Lifetime == AssemblyLifetime.Initialized)
                    throw new InvalidOperationException($"[{tag}] Assembly already initialized");

                if (assembly.Lifetime < AssemblyLifetime.Loaded)
                    throw new InvalidOperationException($"[{tag}] Assembly must be loaded before initialization");

                if (assembly.Lifetime == AssemblyLifetime.Disposed)
                    throw new InvalidOperationException($"[{tag}] Assembly must not be disposed for initialization");
            }

            foreach (var initializer in initializers)
                initializer.Initialize(assembly);

            logger.LogInformation("[{tag}] Assembly initialized", tag);

            assembly.SetInitialized();
        }
    }
}
