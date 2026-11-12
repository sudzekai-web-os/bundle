namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class HandlerMetadata
    {
        public HandlerMetadata() { }

        public HandlerMetadata(string name, string displayName, string[] tags, string summary)
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

        public string GroupName { get; init; } = "v1";

        public string Description { get; init; } = string.Empty;

        public MetadataRequestBody? RequestBody { get; init; }

        public MetadataParameter[] Parameters { get; init; } = [];

        public MetadataResponse[] Responses { get; init; } = [];
    }
}
