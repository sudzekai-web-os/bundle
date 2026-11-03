
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ImageJpegDataResolver : BinaryDataResolver
    {
        public override ContentType ContentType => ContentType.ImageJpeg;

        protected override string FormatName => "jpeg";
        protected override ReadOnlySpan<byte> Signature => [0xFF, 0xD8, 0xFF];
    }
}