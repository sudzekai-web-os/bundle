namespace Sudzekai.WebOs.Core
{
    public class HandlerRoutingInfo
    {
        public HandlerRoutingInfo(int priority, string[] hosts)
        {
            Priority = priority;
            Hosts = hosts;
        }

        public int Priority { get; }

        public string[] Hosts { get; }
    }
}
