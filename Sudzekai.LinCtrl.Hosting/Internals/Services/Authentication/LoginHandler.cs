using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class LoginHandler(LoginCommandDispatcher dispatcher)
    {
        public const string Pattern = "/auth/login";
        private const int _maxBodyBytes = 8192;

        public async Task HandleAsync(HttpContext context)
        {
            var request = context.Request;
            if (!request.HasJsonContentType())
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                return;
            }

            if (request.ContentLength > _maxBodyBytes)
            {
                context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                return;
            }

            var bodyLimit = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (bodyLimit is { IsReadOnly: false } &&
                (bodyLimit.MaxRequestBodySize is null || bodyLimit.MaxRequestBodySize > _maxBodyBytes))
                bodyLimit.MaxRequestBodySize = _maxBodyBytes;

            LoginCommand? command;
            try
            {
                var buffer = new byte[_maxBodyBytes + 1];
                var length = await request.Body.ReadAtLeastAsync(buffer, buffer.Length, false, context.RequestAborted);
                if (length > _maxBodyBytes)
                {
                    context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                    return;
                }

                command = JsonSerializer.Deserialize(buffer.AsSpan(0, length), HostingContext.Default.LoginCommand);
            }
            catch (BadHttpRequestException ex) when (ex.StatusCode == StatusCodes.Status413PayloadTooLarge)
            {
                context.Response.StatusCode = ex.StatusCode;
                return;
            }
            catch (JsonException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (command is null || string.IsNullOrWhiteSpace(command.Username) ||
                command.Username.Length > 191 || string.IsNullOrEmpty(command.Password) || command.Password.Length > 1024)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var token = await dispatcher.DispatchAsync(command, context.RequestAborted);
            if (token is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.Response.Headers.CacheControl = "no-store";
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, token, HostingContext.Default.LoginToken, context.RequestAborted);
        }
    }
}
