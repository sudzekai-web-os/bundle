using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class AssemblyVTableGetHttpRequestHandlersDelegateValidator : IAssemblyVTableFieldValidator
    {
        private readonly ILogger<IAssemblyVTableFieldValidator> _logger;

        public AssemblyVTableGetHttpRequestHandlersDelegateValidator(ILogger<IAssemblyVTableFieldValidator> logger)
        {
            _logger = logger;
        }

        public void ThrowIfInvalid(AssemblyVTable vtable)
        {
            if (vtable.GetHttpRequestHandlersDelegate is null)
                throw new InvalidOperationException($"{nameof(vtable.GetHttpRequestHandlersDelegate)} must not be null");

            _logger.LogInformation("{field} delegate is present", nameof(vtable.GetHttpRequestHandlersDelegate));
        }
    }
}
