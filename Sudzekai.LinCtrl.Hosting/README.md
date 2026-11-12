# Sudzekai.LinCtrl.Hosting

Интеграция с ASP.NET Core для загрузки плагинов Native AOT, регистрации HTTP-маршрутов и управления запуском и выгрузкой плагинов.

```sh
dotnet add package Sudzekai.LinCtrl.Hosting --version 0.5.1
```

Требуется .NET 10.

## Конфигурация плагинов

При загрузке плагин получает снимок полной конфигурации хоста, включая значения из JSON-файлов, переменных окружения и аргументов запуска. Например, `Storage:Path` доступен через `builder.Configuration["Storage:Path"]`. Тот же экземпляр `IConfiguration` зарегистрирован в DI плагина. Снимок включает секреты из конфигурации хоста.

В ABI 4 дескриптор обработчика передаёт флаг фильтрации, а отдельный делегат передаёт фрагмент OpenAPI. Собирайте хост и плагины с совместимыми версиями SDK.

## MySQL и вход

Примените [схему MySQL](https://github.com/sudzekai-web-os/bundle/blob/main/Sudzekai.LinCtrl.Hosting.Application/Database/schema.sql) от имени администратора БД. При запуске хост добавляет недостающие роли с идентификаторами из `ClientRole` и проверяет соответствие идентификаторов и имён.

В секции `LinCtrlDatabase` укажите `Host`, `Database`, `Username` и `Password`. В секции `Jwt` укажите `Secret` (минимум 32 байта UTF-8), `Issuer` и `Audience`. Пример хоста также читает `local/appsettings.json`.

`AddLinCtrlInfrastructure()` включает авторизацию при наличии адреса БД или секрета JWT. `MapLinCtrlInfrastructure()` регистрирует `POST /auth/login`. Передайте `username` и `password` в JSON. При успешном входе поля `token`, `tokenType`, `expiresIn`, `id`, `roleId` и `username` находятся в `data`. Передавайте JWT в заголовке `Authorization: Bearer <токен>`. Заблокированный пользователь получает HTTP 403 с верными учётными данными и теряет доступ с ранее выданным токеном.

`IHashService.Hash` формирует хэш Argon2id для `users.password_hash`. Чтобы заменить MySQL, зарегистрируйте свои `IUserRepository` и `IRoleRepository` после `AddLinCtrlInfrastructure()`.

При Native AOT с Connector/NET добавьте `MySql.Data` и `System.Configuration.ConfigurationManager` в `TrimmerRootAssembly` исполняемого проекта, как в `LinCtrlHost`. При сборке Connector/NET возможны предупреждения об обрезке и AOT.

## Ответы и OpenAPI

Фильтрация включена по умолчанию для `application/json`. Успешный ответ имеет вид `{"success":true,"data":...}`. Для ошибки используйте `HttpResponseDto.CreateError(statusCode, code, message)`. Ответ имеет вид `{"success":false,"error":{"code":...,"message":"..."}}`. Если бизнес-ошибка отсутствует, используются HTTP-статус и его стандартное описание.

Текст, файлы и ответы без содержимого передаются в исходном формате. Для конкретного обработчика фильтрацию можно отключить через `IsFiltered => false`. HEAD и статусы, запрещающие тело, остаются без тела.

Укажите тело запроса, параметры и ответы в `HandlerMetadata` с соответствующим `JsonTypeInfo`. Для текстовых и файловых ответов задайте `MetadataResponse.ContentType` через аргумент `contentType`. Хост и плагины формируют фрагменты общим генератором, а Hosting объединяет их в документе OpenAPI. Схема JSON-ответа учитывает фильтрацию.

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
