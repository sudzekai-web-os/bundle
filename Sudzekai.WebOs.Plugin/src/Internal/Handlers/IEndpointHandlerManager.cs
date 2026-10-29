using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal interface IEndpointHandlerManager
    {
        void Load(IReadOnlyList<Type> handlerTypes);

        IReadOnlyList<IEndpointHandler> Handlers { get; }

        EndpointHandlerDescriptor[] Descriptors { get; }

        void Release();
    }
}