using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class ResponseEnvelopeMetadata
    {
        public static void Apply(EndpointBuilder builder)
        {
            var statusCodes = new HashSet<int>();
            for (var i = 0; i < builder.Metadata.Count; i++)
            {
                if (builder.Metadata[i] is not IProducesResponseTypeMetadata response)
                    continue;

                statusCodes.Add(response.StatusCode);
                builder.Metadata[i] = Create(response.StatusCode);
            }

            if (!statusCodes.Any(ResponseEnvelopeWriter.IsSuccess))
                builder.Metadata.Add(Create(StatusCodes.Status200OK));
            if (!statusCodes.Any(code => code is >= 400 and < 500))
                builder.Metadata.Add(Create(StatusCodes.Status400BadRequest));
            if (statusCodes.Add(StatusCodes.Status500InternalServerError))
                builder.Metadata.Add(Create(StatusCodes.Status500InternalServerError));
        }

        private static ProducesResponseTypeMetadata Create(int statusCode)
        {
            if (!ResponseEnvelopeWriter.AllowsBody(statusCode))
                return new ProducesResponseTypeMetadata(statusCode, typeof(void), []);

            var type = ResponseEnvelopeWriter.IsSuccess(statusCode)
                ? typeof(SuccessResponseEnvelope)
                : typeof(ErrorResponseEnvelope);
            return new ProducesResponseTypeMetadata(statusCode, type, ["application/json"]);
        }
    }
}
