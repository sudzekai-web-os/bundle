using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed record SuccessResponseEnvelope(
        [property: JsonPropertyName("success"), JsonRequired] bool Success,
        [property: JsonPropertyName("data"), JsonRequired] JsonElement Data);
}
