using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Objects;

namespace Sudzekai.LinCtrl.Hosting
{
    public class PluginInfo
    {
        internal PluginInfo(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public PluginLifetime Lifetime { get; private set; }

        public string DirectoryPath
        {
            get
            {
                ThrowIfDisposed();
                return field;
            }
            internal set
            {
                ThrowIfDisposed();
                field = value;
            }
        }

        public PluginManifest Manifest
        {
            get
            {
                ThrowIfDisposed();

                if (field == null)
                    throw new InvalidOperationException("Manifest was null");

                return field;
            }
            internal set
            {
                ThrowIfDisposed();
                field = value;
            }
        }

        private IReadOnlyList<AssemblyInfo> _assemblies = [];

        internal IReadOnlyList<AssemblyInfo> LoadedAssemblies => _assemblies;

        public IReadOnlyList<AssemblyInfo> Assemblies
        {
            get
            {
                ThrowIfOnlyCreated();
                ThrowIfDisposed();
                return _assemblies;
            }
            internal set
            {
                ThrowIfDisposed();
                _assemblies = value;
            }
        }

        internal void SetLoaded()
            => Lifetime = PluginLifetime.Loaded;

        internal void SetInitialized()
            => Lifetime = PluginLifetime.Initialized;

        internal void SetDisposed()
        {
            _assemblies = [];
            Lifetime = PluginLifetime.Disposed;
        }

        private void ThrowIfDisposed()
            => ObjectDisposedException.ThrowIf(Lifetime == PluginLifetime.Disposed, this);

        private void ThrowIfOnlyCreated()
        {
            if (Lifetime == PluginLifetime.Created)
                throw new InvalidOperationException("Plugin was neither loaded nor initialized.");
        }

        internal string GetTag()
            => Path.GetFileName(DirectoryPath);
    }
}
