# Sudzekai.LinCtrl.Hosting.Application

Команды авторизации, репозитории MySQL и хэширование паролей Argon2id для хоста LinCtrl.

```sh
dotnet add package Sudzekai.LinCtrl.Hosting.Application --version 0.1.0
```

Требуется .NET 10.

Контракты `IUserRepository` и `IRoleRepository` позволяют подменять репозитории. `AddLinCtrlHashService()` отдельно регистрирует общий `IHashService` как singleton. Схема базы данных находится в [Database/schema.sql](https://github.com/sudzekai-web-os/bundle/blob/main/Sudzekai.LinCtrl.Hosting.Application/Database/schema.sql).

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
