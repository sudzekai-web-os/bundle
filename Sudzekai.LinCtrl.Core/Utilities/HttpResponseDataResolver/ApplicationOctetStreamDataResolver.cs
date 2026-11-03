
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ApplicationOctetStreamDataResolver : BinaryDataResolver
    {
        public override ContentType ContentType => ContentType.ApplicationOctetStream;

        protected override string FormatName => "binary";

        protected override ReadOnlySpan<byte> Signature => default;

        protected override bool HasSignature(ReadOnlySpan<byte> bytes) => true;
    }
}