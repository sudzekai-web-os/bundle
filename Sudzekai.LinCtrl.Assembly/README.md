# Sudzekai.LinCtrl.Assembly

SDK for building Native AOT plugins with dependency injection, HTTP handlers, delegated logging, and generated ABI exports.

```sh
dotnet add package Sudzekai.LinCtrl.Assembly --version 0.1.0
```

Requires .NET 10 for runtime libraries and a compatible .NET 10 SDK for plugin generation.

The SDK includes the `Sudzekai.LinCtrl.Assembly.Generator` analyzer, which supplies the `[PluginAssembly]` entry-point generator during compilation. A separate generator package reference is not required.

Source: [sudzekai-web-os/bundle](https://github.com/sudzekai-web-os/bundle).
License: Apache-2.0.
