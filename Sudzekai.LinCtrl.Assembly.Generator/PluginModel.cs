namespace Sudzekai.LinCtrl.Assembly.Generator;

internal readonly record struct PluginAssemblyModel(
    string TypeName,
    string FullTypeName,
    string? Namespace,
    bool IsPartial,
    bool IsPublic,
    bool ImplementsIPluginAssembly
);