# Sudzekai.LinCtrl.Assembly.Generator

Roslyn source generator for Native AOT plugin entry points and ABI function-table exchange in Sudzekai LinCtrl.

```sh
dotnet add package Sudzekai.LinCtrl.Assembly.Generator --version 0.1.0
```

Requires .NET 10 for runtime libraries and a compatible .NET 10 SDK for plugin generation.

This is a compiler analyzer package. Its generator is installed under `analyzers/dotnet/cs`; it is not a runtime library. The plugin SDK already includes this generator, so SDK consumers do not need to install this standalone package.

Source: [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle).
License: Apache-2.0.
