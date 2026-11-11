using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed record ErrorResponseEnvelope(
        [property: JsonPropertyName("success"), JsonRequired] bool Success,
        [property: JsonPropertyName("error"), JsonRequired] ResponseEnvelopeError Error);
}
