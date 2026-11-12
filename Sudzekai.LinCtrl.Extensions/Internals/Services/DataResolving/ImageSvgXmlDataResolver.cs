
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions.Internals.Services.DataResolving
{
    internal sealed class ImageSvgXmlDataResolver : TextDataResolver
    {
        public override ResponseContentType ContentType => ResponseContentType.ImageSvgXml;

        protected override byte[] ToUtf8Bytes<T>(T data)
        {
            var bytes = base.ToUtf8Bytes(data);
            if (bytes.Length == 0)
                return bytes;

            int i = 0;

            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                i = 3;

            while (i < bytes.Length && bytes[i] is (byte)' ' or (byte)'\t' or (byte)'\r' or (byte)'\n')
                i++;

            if (i >= bytes.Length || bytes[i] != (byte)'<')
                throw new InvalidOperationException(
                    "Recieved data was not correct. (not an svg/xml format)");

            return bytes;
        }
    }
}