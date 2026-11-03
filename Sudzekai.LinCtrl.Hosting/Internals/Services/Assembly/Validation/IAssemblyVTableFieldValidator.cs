using Sudzekai.LinCtrl.Core;
namespace Sudzekai.LinCtrl.Hosting 
{
    internal interface IAssemblyVTableFieldValidator
    {
        void ThrowIfInvalid(AssemblyVTable vTable);
    }
}
