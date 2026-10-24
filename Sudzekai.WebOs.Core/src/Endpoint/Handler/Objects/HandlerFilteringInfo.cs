namespace Sudzekai.WebOs.Core
{
    public class HandlerFilteringInfo
    {
        public HandlerFilteringInfo(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; } = true;
    }
}
