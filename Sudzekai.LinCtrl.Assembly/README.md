# Sudzekai.LinCtrl.Assembly

SDK для разработки плагинов Native AOT с внедрением зависимостей, HTTP-обработчиками, журналированием через хост и генерацией экспортов ABI.

```sh
dotnet add package Sudzekai.LinCtrl.Assembly --version 0.5.0
```

Требуется .NET 10.

SDK включает анализатор `Sudzekai.LinCtrl.Assembly.Generator`. Он генерирует точку входа из `[PluginAssembly]` при сборке проекта. Достаточно ссылки на SDK.

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
