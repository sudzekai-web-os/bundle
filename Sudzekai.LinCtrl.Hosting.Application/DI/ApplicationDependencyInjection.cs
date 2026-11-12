using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    public static class ApplicationDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public void AddApplicationServices(string connectionString)
            {
                services.AddSingleton<IDbContext>(new AppDbContext(connectionString));
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<ICommandDispatcher, CommandDispatcher>();
                services.AddTransient<ICommandHandler<LoginCommand, LoginResult>, LoginCommandHandler>();
                services.AddLinCtrlHashService();
            }

            public void AddLinCtrlHashService()
            {
                services.AddSingleton<IHashService, HashService>();
            }
        }
    }
}
