using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal unsafe class AssemblyInfo
    {
        public AssemblyInfo(
            AssemblyVTable vTable,
            AssemblyVTableDescriptor* vTableDescriptor,
            nint handle)
        {
            VTable = vTable;
            VTableDescriptor = vTableDescriptor;
            Handle = handle;
        }

        public bool IsDisposed
        {
            get;
            set
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                field = value;
            }
        } = false;

        public bool IsInitialized
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                return field;
            }
            set
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                field = value;
            }
        }

        public AssemblyVTableDescriptor* VTableDescriptor
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                return field;
            }
        }

        public AssemblyVTable VTable
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                return field;
            }
        }

        public nint Handle
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));
                return field;
            }
            private set;
        }

        private IHttpRequestHandler[]? _handlers;

        public IHttpRequestHandler[] GetHttpRequestHandlers()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, nameof(AssemblyInfo));

            _handlers ??= VTable.GetHttpRequestHandlersDelegate!.Invoke();

            return _handlers;
        }
    }
}
