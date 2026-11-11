using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public sealed record RequestHandlerMetadata(
        HttpRequestMethod HttpMethod,
        string Pattern,
        string Summary,
        string Description,
        int[] ResponseStatusCodes,
        bool AcceptsJson = false,
        string[]? IntegerQueryParameters = null,
        string? Group = null,
        string? ResponseDataSchema = null);
}
