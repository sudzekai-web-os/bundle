using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Internals.Extensions.Abi;
using Sudzekai.LinCtrl.Hosting.Objects;
using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    [JsonSerializable(typeof(PluginManifest))]
    internal partial class HostingContext : JsonSerializerContext
    {
        private static HostVTable _vTable = new() { AbiVersion = LinCtrlContext.AbiVersion };

        public unsafe static HostVTableDescriptor* VTableDescriptorPointer = CoTaskAllocator.Alloc(HostVTableAllocator.Alloc(_vTable));
    }
}