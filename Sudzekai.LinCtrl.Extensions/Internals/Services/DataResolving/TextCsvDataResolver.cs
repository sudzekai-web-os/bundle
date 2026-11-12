
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions.Internals.Services.DataResolving
{
    internal sealed class TextCsvDataResolver : TextDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.TextCsv;
    }
}