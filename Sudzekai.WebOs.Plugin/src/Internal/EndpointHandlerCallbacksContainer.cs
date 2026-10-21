using System.Collections.Concurrent;

namespace Sudzekai.WebOs.Plugin
{
    internal class EndpointHandlerCallbacksContainer
    {
        private static readonly ConcurrentDictionary<nint, Delegate> _callbacks = new();

        public static void Fix(nint handleFunc, Delegate describedFunc)
            => _callbacks[handleFunc] = describedFunc;

        public static void Free(nint handleFunc)
        {
            if (handleFunc != nint.Zero)
                _callbacks.TryRemove(handleFunc, out _);
        }
    }
}
