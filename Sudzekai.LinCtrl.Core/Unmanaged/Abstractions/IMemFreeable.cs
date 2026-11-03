namespace Sudzekai.LinCtrl.Core 
{
    public interface IMemFreeable
    {
        bool IsAllocated { get; }
        
        void Free();
    }
}
