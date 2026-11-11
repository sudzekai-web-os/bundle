using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed record ResponseEnvelopeError(
        [property: JsonPropertyName("code"), JsonRequired] int Code,
        [property: JsonPropertyName("message"), JsonRequired] string Message);
}
