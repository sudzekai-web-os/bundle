using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Sudzekai.LinCtrl.Hosting.Application;
using System.Globalization;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Authentication.Services
{
    internal class JwtMiddleware(IUserRepository users) : IMiddleware
    {
        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            if (context.User.Identity?.IsAuthenticated != true
                || context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await next(context);
                return;
            }

            var username = context.User.FindFirst("name")?.Value;
            var subject = context.User.FindFirst("sub")?.Value;
            var role = context.User.FindFirst("role")?.Value;

            var user = username == null ? null : await users.FindByUsernameAsync(username, context.RequestAborted);

            if (user == null || user.IsBlocked || user.Id.ToString(CultureInfo.InvariantCulture) != subject
                || user.Role.ToString() != role)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await next(context);
        }
    }
}
