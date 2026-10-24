namespace Sudzekai.WebOs.Core
{
    public class HandlerOpenApiInfo
    {
        public HandlerOpenApiInfo(string name, string displayName, string[] tags, string summary, string groupName)
        {
            Name = name;
            DisplayName = displayName;
            Tags = tags;
            Summary = summary;
            GroupName = groupName;
        }

        public string Name { get; }

        public string DisplayName { get; }

        public string[] Tags { get; }

        public string Summary { get; }

        public string GroupName { get; }
    }
}
