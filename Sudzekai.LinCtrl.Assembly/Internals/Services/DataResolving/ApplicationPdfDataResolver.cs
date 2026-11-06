
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ApplicationPdfDataResolver : BinaryDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ApplicationPdf;

        protected override string FormatName => "pdf";
        protected override ReadOnlySpan<byte> Signature => "%PDF"u8;
    }
}