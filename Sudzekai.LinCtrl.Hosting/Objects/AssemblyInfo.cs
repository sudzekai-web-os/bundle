using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    public class AssemblyInfo
    {
        internal AssemblyInfo(string fullPath)
        {
            FullPath = fullPath;
        }

        public AssemblyLifetime Lifetime { get; private set; }

        // loading

        public string FullPath
        {
            get
            {
                ThrowIfDisposed();
                return field;
            }
            internal set;
        }

        internal bool HasVTable => _vTable != null;

        private AssemblyVTable? _vTable;

        internal AssemblyVTable VTable
        {
            get
            {
                ThrowIfDisposed();

                return _vTable ?? throw new InvalidOperationException("VTable was null");
            }
            set => _vTable = value;
        }

        public PluginInfo PluginInfo
        {
            get
            {
                ThrowIfOnlyCreated();
                ThrowIfDisposed();

                if (field == null)
                    throw new InvalidOperationException("PluginInfo was null");

                return field;
            }
            internal set;
        }

        public IReadOnlyList<IRequestHandler> Handlers
        {
            get
            {
                ThrowIfOnlyCreated();
                ThrowIfDisposed();
                return field;
            }
            internal set;
        } = [];

        internal void SetLoaded()
            => Lifetime = AssemblyLifetime.Loaded;
        
        internal void SetInitialized()
            => Lifetime = AssemblyLifetime.Initialized;

        internal void SetDisposed()
        {
            Handlers = [];
            _vTable = null;
            Lifetime = AssemblyLifetime.Disposed;
        }

        private void ThrowIfDisposed()
            => ObjectDisposedException.ThrowIf(Lifetime == AssemblyLifetime.Disposed, this);

        private void ThrowIfOnlyCreated()
        {
            if (Lifetime == AssemblyLifetime.Created)
                throw new InvalidOperationException($"Assembly was neither loaded nor initialized. (path: {FullPath})");
        }

        internal string GetTag()
            => Path.GetFileName(Path.GetDirectoryName(FullPath)) + "/" + Path.GetFileNameWithoutExtension(FullPath);
    }
}
