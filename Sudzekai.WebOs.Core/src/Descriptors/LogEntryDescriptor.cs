namespace Sudzekai.WebOs.Core
{
    public struct LogEntryDescriptor : IDisposable
    {
        public int LogLevel;
        public nint Message; public int MessageLength;
        public nint Category; public int CategoryLength;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Message);
            AllocationUtilities.FreeIfAllocated(ref Category);
        }
    }
}
