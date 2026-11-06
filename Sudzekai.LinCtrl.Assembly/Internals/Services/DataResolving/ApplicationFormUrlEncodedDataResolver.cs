using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ApplicationFormUrlEncodedDataResolver : TextDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ApplicationFormUrlEncoded;
    }
}