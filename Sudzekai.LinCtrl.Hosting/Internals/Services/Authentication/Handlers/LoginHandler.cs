using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Application;
using System.Net;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class LoginHandler(ICommandDispatcher dispatcher, IJwtTokenService tokens) : IRequestHandler
    {
        public string Pattern => "auth/login";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

        public HandlerMetadata Metadata { get; } = new(
            "LinCtrl.Login",
            "Вход в систему",
            ["auth"],
            "Вход в систему")
        {
            Description = "Выдаёт JWT и сведения о пользователе.",
            RequestBody = new(LinCtrlContext.GetTypeInfo<LoginRequest>()),
            Responses = [
                new(200, LinCtrlContext.GetTypeInfo<LoginResponse>()),
                new(400),
                new(401),
                new(403),
                new(413),
                new(415),
                new(429, contentType: ResponseContentType.None)
            ]
        };

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            if (request.ContentType != ResponseContentType.ApplicationJson)
                return HttpResponseDto.CreateError(HttpStatusCode.UnsupportedMediaType, 415, "Требуется Content-Type application/json.");

            LoginRequest? loginRequest;

            try
            {
                loginRequest = JsonSerializer.Deserialize(request.Body, LinCtrlContext.GetTypeInfo<LoginRequest>());
            }
            catch (JsonException)
            {
                return HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 400, "Укажите имя пользователя и пароль в теле JSON.");
            }

            if (loginRequest is null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                return HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 400, "Укажите имя пользователя и пароль в теле JSON.");

            var result = dispatcher.DispatchAsync<LoginCommand, LoginResult>(
                new LoginCommand(loginRequest.Username, loginRequest.Password)
            ).GetAwaiter().GetResult();

            if (result.IsBlocked)
                return HttpResponseDto.CreateError(HttpStatusCode.Forbidden, 403, "Пользователь заблокирован");

            if (result.User == null)
                return HttpResponseDto.CreateError(HttpStatusCode.Unauthorized, 401, "Неверное имя пользователя или пароль");

            return HttpResponseDto.CreateJson(HttpStatusCode.OK, tokens.Issue(result.User));
        }
    }
}
