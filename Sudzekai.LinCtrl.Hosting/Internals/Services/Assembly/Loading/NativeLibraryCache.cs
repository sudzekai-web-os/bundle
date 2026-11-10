using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class NativeLibraryCache
    {
        private static readonly ConcurrentDictionary<string, Lazy<nint>> _handles = new(
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        // Native AOT libraries stay loaded until process exit; retain one handle per path.
        public static nint Load(string fullPath)
            => _handles.GetOrAdd(Path.GetFullPath(fullPath), path => new(() => NativeLibrary.Load(path))).Value;
    }
}
