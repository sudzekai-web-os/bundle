
namespace Sudzekai.LinCtrl.Core 
{
    internal sealed class ImageWebpDataResolver : BinaryDataResolver
    {
        public override ContentType ContentType => ContentType.ImageWebp;

        protected override string FormatName => "webp";
        protected override ReadOnlySpan<byte> Signature => "RIFF"u8;

        protected override bool HasSignature(ReadOnlySpan<byte> bytes)
            => bytes.Length >= 12
               && bytes[..4].SequenceEqual("RIFF"u8)
               && bytes[8..12].SequenceEqual("WEBP"u8);
    }
}