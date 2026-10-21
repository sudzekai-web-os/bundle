using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HeaderEntryDescriptor : IDisposable
    {
        public nint Key; public int KeyLength;

        public nint* Values; public int ValuesCount;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Key);

            if (Values != null)
            {
                AllocationUtilities.FreeStringArray(Values, ValuesCount);
                Values = null;
            }
        }
    }
}
