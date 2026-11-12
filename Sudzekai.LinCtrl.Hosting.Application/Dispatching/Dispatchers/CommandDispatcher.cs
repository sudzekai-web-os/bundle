using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal sealed class CommandDispatcher(IServiceProvider services) : ICommandDispatcher
    {
        public Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>
            => services.GetRequiredService<ICommandHandler<TCommand, TResult>>().HandleAsync(command, cancellationToken);
    }
}
