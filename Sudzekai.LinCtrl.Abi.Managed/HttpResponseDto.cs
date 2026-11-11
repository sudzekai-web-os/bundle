using Sudzekai.LinCtrl.Core;
using System.Net;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class HttpResponseDto
    {
        public ResponseContentType ContentType;

        public HttpStatusCode StatusCode;

        public byte[] Data = [];
        public ResponseError? Error;
    }
}
