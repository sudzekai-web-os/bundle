# Sudzekai.LinCtrl.Extensions

Расширения для создания HTTP-ответов LinCtrl и преобразования типов содержимого в MIME.

```sh
dotnet add package Sudzekai.LinCtrl.Extensions --version 0.2.0
```

Требуется .NET 10.

Методы `HttpResponseExtensions` доступны через `Sudzekai.LinCtrl.Assembly.Extensions`. Для сериализации JSON зарегистрируйте свой контекст в `LinCtrlContext`. Расширения `ToMimeString` и `FromMimeString` доступны через `Sudzekai.LinCtrl.Extensions`.

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
