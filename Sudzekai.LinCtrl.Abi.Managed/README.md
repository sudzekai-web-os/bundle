# Sudzekai.LinCtrl.Abi.Managed

Управляемые DTO запросов и ответов, обратные вызовы плагинов и контракты обработчиков ABI LinCtrl.

```sh
dotnet add package Sudzekai.LinCtrl.Abi.Managed --version 0.5.0
```

Требуется .NET 10.

Флаг `IRequestHandler.IsFiltered` по умолчанию включён. Hosting применяет обёртку к ответам `application/json`. Запросы, параметры и ответы описываются через `HandlerMetadata` и `JsonTypeInfo`.

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
