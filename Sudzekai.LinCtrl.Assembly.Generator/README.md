# Sudzekai.LinCtrl.Assembly.Generator

Генератор исходного кода Roslyn для точек входа плагинов Native AOT и обмена таблицами функций ABI LinCtrl.

```sh
dotnet add package Sudzekai.LinCtrl.Assembly.Generator --version 0.2.0
```

Требуется .NET 10.

Анализатор устанавливается в `analyzers/dotnet/cs` и используется компилятором. Пакет `Sudzekai.LinCtrl.Assembly` уже включает его.

Исходный код доступен в [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle). Лицензия Apache-2.0.
