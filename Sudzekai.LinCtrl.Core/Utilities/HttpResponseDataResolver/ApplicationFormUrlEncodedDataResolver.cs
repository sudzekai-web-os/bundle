
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ApplicationFormUrlEncodedDataResolver : TextDataResolver
    {
        public override ContentType ContentType => ContentType.ApplicationFormUrlEncoded;
    }
}