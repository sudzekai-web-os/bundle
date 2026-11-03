using System.Globalization;
using System.Text;

namespace Sudzekai.LinCtrl.Core 
{
    internal abstract class TextDataResolver : IHttpResponseDataResolver
    {
        public abstract ContentType ContentType { get; }

        public HttpResponse Resolve<T>(HttpResponse response, T data)
        {
            response.Data = ToUtf8Bytes(data);
            return response;
        }

        protected virtual byte[] ToUtf8Bytes<T>(T data)
        {
            var text = ToText(data);
            return text.Length == 0 ? [] : Encoding.UTF8.GetBytes(text);
        }

        protected virtual string ToText<T>(T data) => data switch
        {
            null => "",
            string s => s,
            byte[] b => Encoding.UTF8.GetString(b),
            ReadOnlyMemory<byte> m => Encoding.UTF8.GetString(m.Span),
            Memory<byte> m => Encoding.UTF8.GetString(m.Span),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture) ?? "",
            _ => throw new NotSupportedException(
                $"Not suported data type. (recieved: {typeof(T).FullName})"),
        };
    }
}