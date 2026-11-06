using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class TextPlainDataResolver : TextDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.TextPlain;
    }
}