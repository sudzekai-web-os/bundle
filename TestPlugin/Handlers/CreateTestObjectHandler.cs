using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using System.Text;
using System.Text.Json;
using TestPlugin.Context;
using TestPlugin.Object;

namespace TestPlugin.Handlers
{
    internal class CreateTestObjectHandler : IEndpointHandler
    {
        public string Pattern => "object";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Post;

        public HandlerFilteringInfo? FilteringInfo { get; } = new(true);

        public HandlerOpenApiInfo? OpenApiInfo { get; } = new(
            "createTestObject",
            "Create test object",
            ["object"],
            "Создаёт тестовый объект")
        {
            Description = "Принимает тестовый объект в теле запроса и возвращает его в обёртке ответа (filtered).",
            RequestBody = new(
                PluginJsonContext.Default.TestObject,
                "Тестовый объект для создания",
                required: true),
            Responses =
            [
                new(200, PluginJsonContext.Default.TestObject, "Объект создан"),
                new(400, PluginJsonContext.Default.String, "Некорректные данные")
            ]
        };

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            if (request.Body is null || request.Body.Length == 0)
                return Task.FromResult(HandlerResult.BadRequest()
                    .WithData("Тело запроса не может быть пустым")
                    .AsJson(PluginJsonContext.Default.String));

            try
            {
                var json = Encoding.UTF8.GetString(request.Body);

                var obj = JsonSerializer.Deserialize<TestObject>(json, PluginJsonContext.Default.TestObject);

                if (obj is null)
                    return Task.FromResult(HandlerResult.BadRequest()
                        .WithData("Некорректные данные")
                        .AsJson(PluginJsonContext.Default.String));

                return Task.FromResult(HandlerResult.Ok()
                    .WithData(obj)
                    .AsJson(PluginJsonContext.Default.TestObject));
            }
            catch (JsonException)
            {
                return Task.FromResult(HandlerResult.BadRequest()
                    .WithData("Некорректный JSON")
                    .AsJson(PluginJsonContext.Default.String));
            }
        }
    }
}