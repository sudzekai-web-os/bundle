
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ApplicationPdfDataResolver : BinaryDataResolver
    {
        public override ContentType ContentType => ContentType.ApplicationPdf;

        protected override string FormatName => "pdf";
        protected override ReadOnlySpan<byte> Signature => "%PDF"u8;
    }
}