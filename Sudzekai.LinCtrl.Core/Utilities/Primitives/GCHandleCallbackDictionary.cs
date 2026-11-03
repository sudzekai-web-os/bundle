using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static class GCHandleCallbackDictionary
    {
        private static readonly ConcurrentDictionary<GCHandle, byte> _handles = [];

        public static GCHandle Alloc(object obj)
        {
            GCHandle handle = GCHandle.Alloc(obj);

            try
            {
                _handles[handle] = 0;
            }
            catch
            {
                handle.Free();
                throw;
            }

            return handle;
        }

        public static void Alloc(GCHandle handle)
        {
            try
            {
                _handles[handle] = 0;
            }
            catch
            {
                handle.Free();
                throw;
            }
        }

        public static void Free(ref GCHandle handle)
        {
            if (!_handles.TryRemove(handle, out _))
                return;

            if (!handle.IsAllocated || handle == default)
                return;

            handle.Free();
            handle = default;
        }
    }
}
