namespace Sudzekai.LinCtrl.Hosting 
{
    public class PluginInfo
    {
        internal PluginInfo(PluginManifest mainfest, IReadOnlyList<AssemblyInfo> assemblyInfos)
        {
            Name = mainfest.Name;
            Version = mainfest.Version;
            Summary = mainfest.Summary;

            Assemblies = assemblyInfos;
        }

        internal bool IsDisposed
        {
            get;
            set
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                field = value;
            }
        } = false;

        public bool IsInitialized
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                return field;
            }
            internal set;
        } = false;

        public string Name
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                return field;
            }
        } = string.Empty;

        public string Version
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                return field;
            }
        } = string.Empty;

        public string Summary
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                return field;
            }
        } = string.Empty;

        internal IReadOnlyList<AssemblyInfo> Assemblies
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(PluginInfo));
                return field;
            }
        } = [];
    }
}
