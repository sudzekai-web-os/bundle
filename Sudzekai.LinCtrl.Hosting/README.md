# Sudzekai.LinCtrl.Hosting

ASP.NET Core integration for loading Native AOT plugins, registering HTTP endpoints, and coordinating plugin startup and disposal.

```sh
dotnet add package Sudzekai.LinCtrl.Hosting --version 0.1.0
```

Requires .NET 10 for runtime libraries and a compatible .NET 10 SDK for plugin generation.

Source: [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle).
License: Apache-2.0.


## Plugin configuration

Each plugin receives a snapshot of the complete host configuration at load time, including settings from JSON files, environment variables and command-line arguments. For example, the host key `Storage:Path` is available as `builder.Configuration["Storage:Path"]`. The same `IConfiguration` is registered in the plugin DI container. The snapshot includes any secrets present in the host configuration.

This changes the host table layout in ABI version 2. Rebuild the host and plugins together.

## MySQL and login

Apply [Database/schema.sql](https://github.com/sudzekai-web-os/bundle/blob/dev/Sudzekai.LinCtrl.Hosting/Database/schema.sql) as a database administrator. The application account needs data permissions on the LinCtrl schema. At startup the host seeds the missing `ClientRole` rows and reports any existing ID/name mismatch.

Configure `LinCtrlDatabase:Host`, `Database`, `Username`, and `Password` under the `LinCtrlDatabase` section. Configure `Jwt:Secret` (at least 32 UTF-8 bytes), `Jwt:Issuer`, and `Jwt:Audience` under `Jwt`. The example host also reads `local/appsettings.json` for credentials.

`AddLinCtrlInfrastructure()` enables authentication when a database host or JWT secret is configured. `MapLinCtrlInfrastructure()` maps `POST /auth/login` with JSON fields `username` and `password`. A valid account receives `accessToken`, `tokenType`, and `expiresIn`. Use the bearer token for endpoints configured to require authorization.

`PasswordHashService.Hash` produces the Argon2id string stored in `users.password_hash`. Register alternate `IUserRepository` and `IRoleRepository` implementations before calling `AddLinCtrlInfrastructure()` to replace MySQL persistence.

For Native AOT with the default Connector/NET repositories, preserve `MySql.Data` and `System.Configuration.ConfigurationManager` with `TrimmerRootAssembly` in the executable project, as configured in LinCtrlHost. Connector/NET and its dependencies can report trim and AOT analysis warnings.
