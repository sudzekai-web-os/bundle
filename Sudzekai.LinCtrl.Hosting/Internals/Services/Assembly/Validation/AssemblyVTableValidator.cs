using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyVTableValidator(
        IEnumerable<IAssemblyVTableFieldValidator> validators,
        ILogger<AssemblyVTableValidator> logger)
    {
        public void ThrowIfInvalid(AssemblyVTable vTable)
        {
            foreach (var validator in validators)
                validator.ThrowIfInvalid(vTable);

            logger.LogInformation("Assembly VTable is valid");
        }
    }
}
