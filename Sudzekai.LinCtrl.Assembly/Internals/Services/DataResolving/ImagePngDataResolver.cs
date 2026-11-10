
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ImagePngDataResolver : BinaryDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ImagePng;

        protected override string FormatName => "png";
        protected override ReadOnlySpan<byte> Signature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    }
}