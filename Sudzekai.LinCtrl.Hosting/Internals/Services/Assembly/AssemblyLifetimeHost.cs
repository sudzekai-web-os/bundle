using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyLifetimeHost(
        IEnumerable<IAssemblyLoader> loaders,
        IEnumerable<IAssemblyInitializer> initializers,
        ILogger<AssemblyLifetimeHost> logger,
        RequestHandlerLifetimeHost handlers) : IDisposable
    {
        private readonly HashSet<AssemblyInfo> _assemblies = [];

        public AssemblyInfo Load(string fullPath)
        {
            AssemblyInfo assembly = new(fullPath);

            var tag = assembly.GetTag();

            logger.LogInformation("[{tag}] Assembly loading started", tag);

            if (!Path.IsPathFullyQualified(fullPath))
                throw new InvalidOperationException($"[{tag}] Assembly path must be fully qualified");

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"[{tag}] Assembly path not found");

            try
            {
                foreach (var loader in loaders)
                    loader.Load(assembly);
            }
            catch
            {
                DisposeAfterFailure(assembly);
                throw;
            }

            _assemblies.Add(assembly);

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

            try
            {
                foreach (var initializer in initializers)
                    initializer.Initialize(assembly);
            }
            catch
            {
                DisposeAfterFailure(assembly);
                throw;
            }

            logger.LogInformation("[{tag}] Assembly initialized", tag);

            assembly.SetInitialized();
        }

        private void DisposeAfterFailure(AssemblyInfo assembly)
        {
            try
            {
                Dispose(assembly);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception while releasing assembly resources: {message}", ex.Message);
            }
        }

        public void Dispose(AssemblyInfo assembly)
        {
            if (assembly.Lifetime == AssemblyLifetime.Disposed)
                return;

            try
            {
                if (assembly.Lifetime != AssemblyLifetime.Created)
                    handlers.Remove(assembly.Handlers);
            }
            finally
            {
                try
                {
                    if (assembly.HasVTable)
                        assembly.VTable.AssemblyDisposeDelegate();
                }
                finally
                {
                    assembly.SetDisposed();
                    _assemblies.Remove(assembly);
                }
            }
        }

        public void Dispose()
        {
            List<Exception> exceptions = [];
            foreach (var assembly in _assemblies.ToArray())
            {
                try
                {
                    Dispose(assembly);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException("Failed to release assembly resources", exceptions);
        }
    }
}
