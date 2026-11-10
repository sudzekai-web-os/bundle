
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ImageJpegDataResolver : BinaryDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ImageJpeg;

        protected override string FormatName => "jpeg";
        protected override ReadOnlySpan<byte> Signature => [0xFF, 0xD8, 0xFF];
    }
}