
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class TextHtmlDataResolver : TextDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.TextHtml;
    }
}