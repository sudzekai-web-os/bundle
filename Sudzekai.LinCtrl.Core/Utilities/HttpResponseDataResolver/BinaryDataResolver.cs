
namespace Sudzekai.LinCtrl.Core 
{
    internal abstract class BinaryDataResolver : IHttpResponseDataResolver
    {
        public abstract ContentType ContentType { get; }

        protected abstract string FormatName { get; }
        protected abstract ReadOnlySpan<byte> Signature { get; }

        public HttpResponse Resolve<T>(HttpResponse response, T data)
        {
            response.Data = ToBytes(data);
            return response;
        }

        private byte[] ToBytes<T>(T data) => data switch
        {
            null => [],
            byte[] b => ValidateOrThrow(b),
            ReadOnlyMemory<byte> m => ValidateOrThrow(m.Span),
            Memory<byte> m => ValidateOrThrow(m.Span),
            Stream s => ValidateOrThrow(ReadStream(s)),
            _ => throw new NotSupportedException(
                $"Not suported data type. Method supports only: byte[], ReadOnlyMemory<byte>, Memory<byte> or Stream. (recieved: {typeof(T).FullName})"),
        };

        private byte[] ValidateOrThrow(ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
                return [];

            if (!HasSignature(bytes))
                throw new InvalidOperationException(
                    $"Recieved data was not correct. (not a {FormatName} format)");

            return bytes.ToArray();
        }

        protected virtual bool HasSignature(ReadOnlySpan<byte> bytes)
            => bytes.Length >= Signature.Length && bytes[..Signature.Length].SequenceEqual(Signature);

        private static byte[] ReadStream(Stream s)
        {
            if (s.CanSeek && s.Position == 0 && s.Length <= int.MaxValue)
            {
                var buffer = new byte[s.Length];
                s.ReadExactly(buffer);
                return buffer;
            }

            using var ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
        }
    }
}