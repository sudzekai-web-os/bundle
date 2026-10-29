namespace Sudzekai.WebOs.Core
{
    public class HandlerOpenApiInfo
    {
        public HandlerOpenApiInfo() { }

        public HandlerOpenApiInfo(string name, string displayName, string[] tags, string summary)
        {
            Name = name;
            DisplayName = displayName;
            Tags = tags;
            Summary = summary;
        }

        public string Name { get; } = string.Empty;

        public string DisplayName { get; } = string.Empty;

        public string[] Tags { get; } = [];

        public string Summary { get; } = string.Empty;

        public string GroupName { get; } = "v1";

        public string Description { get; init; } = string.Empty;

        public HandlerOpenApiRequestBody? RequestBody { get; init; }

        public HandlerOpenApiResponse[] Responses { get; init; } = [];
    }
}
