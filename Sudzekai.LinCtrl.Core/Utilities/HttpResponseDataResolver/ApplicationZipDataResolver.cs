
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ApplicationZipDataResolver : BinaryDataResolver
    {
        public override ContentType ContentType => ContentType.ApplicationZip;

        protected override string FormatName => "zip";
        protected override ReadOnlySpan<byte> Signature => [0x50, 0x4B, 0x03, 0x04];
    }
}