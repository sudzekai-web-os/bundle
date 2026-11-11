using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Sudzekai.LinCtrl.Hosting.Domain.Authentication;
using System.Text;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class AuthenticationServicesDependencyInjection
    {
        public const string LoginRateLimit = "LinCtrl.Login";

        extension(IServiceCollection services)
        {
            public void AddLinCtrlAuthentication(IConfiguration configuration)
            {
                var secret = Required(configuration, "Secret");
                var key = Encoding.UTF8.GetBytes(secret);
                if (key.Length < 32)
                    throw new InvalidOperationException("Jwt:Secret must contain at least 32 UTF-8 bytes");

                var validation = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Required(configuration, "Issuer"),
                    ValidateAudience = true,
                    ValidAudience = Required(configuration, "Audience"),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    NameClaimType = "name",
                    RoleClaimType = "role",
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = validation;
                });
                services.AddAuthorization();
                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = 429;
                    options.AddConcurrencyLimiter(LoginRateLimit, limiter =>
                    {
                        limiter.PermitLimit = 4;
                        limiter.QueueLimit = 0;
                    });
                });

                services.TryAddSingleton<MySqlConnectionFactory>();
                services.TryAddScoped<IUserRepository, MySqlUserRepository>();
                services.TryAddScoped<IRoleRepository, MySqlRoleRepository>();
                services.TryAddSingleton<PasswordHashService>();
                services.TryAddSingleton(new JwtTokenService(validation));
                services.TryAddTransient<LoginCommandDispatcher>();
                services.TryAddTransient<LoginHandler>();
                services.AddHostedService<RoleInitializer>();
            }
        }

        private static string Required(IConfiguration configuration, string key)
            => configuration[$"Jwt:{key}"] is { Length: > 0 } value
                ? value
                : throw new InvalidOperationException($"Missing configuration: Jwt:{key}");
    }
}
