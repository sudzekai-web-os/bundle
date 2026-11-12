
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions.Internals.Services.DataResolving
{
    internal sealed class ApplicationZipDataResolver : BinaryDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ApplicationZip;

        protected override string FormatName => "zip";
        protected override ReadOnlySpan<byte> Signature => [0x50, 0x4B, 0x03, 0x04];
    }
}