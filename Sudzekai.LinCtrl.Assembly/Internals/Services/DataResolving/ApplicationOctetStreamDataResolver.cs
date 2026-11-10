
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.DataResolving
{
    internal sealed class ApplicationOctetStreamDataResolver : BinaryDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ApplicationOctetStream;

        protected override string FormatName => "binary";

        protected override ReadOnlySpan<byte> Signature => default;

        protected override bool HasSignature(ReadOnlySpan<byte> bytes) => true;
    }
}