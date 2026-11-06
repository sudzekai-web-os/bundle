using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Context
{
    public class AssemblyContext
    {
        public unsafe AssemblyContext(HostVTableDescriptor* hostVTableDescriptorPointer)
        {
            
        }

        public const uint AbiVersion = 1;

        /// <summary>
        /// Делегат функции вызывающейся первой в очереди
        /// </summary>
        public InitializeDelegate? InitializeDelegate
        {
            get;
            init
            {
                field = () =>
                {
                    if (value == null)
                    {
                        return 1;
                    }

                    try
                    {
                        return value();
                    }
                    catch
                    {
                        return 2;
                    }
                };
            }
        }

        /// <summary>
        /// Делегат функции вызывающейся второй в очереди
        /// </summary>
        public DelegateLoggingDelegate? DelegateLoggingDelegate
        {
            get; init
            {
                field = (del) =>
                {
                    if (value == null)
                    {
                        return 1;
                    }

                    try
                    {
                        return value(del);
                    }
                    catch
                    {
                        return 2;
                    }
                };
            }
        }

        /// <summary>
        /// Делегат функции вызывающейся третьей в очереди
        /// </summary>
        public BuildDelegate? BuildDelegate
        {
            get;
            init
            {
                field = () =>
                {
                    if (value == null)
                    {
                        return 1;
                    }

                    try
                    {
                        return value();
                    }
                    catch
                    {
                        return 2;
                    }
                };
            }
        }

        /// <summary>
        /// Делегат функции вызывающейся четвёртой в очереди
        /// </summary>
        public GetHttpRequestHandlersDelegate? GetHttpRequestHandlersDelegate
        {
            get;
            init
            {
                field = () =>
                {
                    if (value == null)
                    {
                        return [];
                    }

                    try
                    {
                        return value();
                    }
                    catch
                    {
                        return [];
                    }
                };
            }
        }

        /// <summary>
        /// Делегат функции вызывающейся пятой в очереди
        /// </summary>
        public GetHttpRequestHandlersMetadataDelegate? GetHttpRequestHandlersMetadataDelegate
        {
            get;
            init
            {
                field = () =>
                {
                    if (value == null)
                    {
                        return string.Empty;
                    }

                    try
                    {
                        return value();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                };
            }
        }

        public unsafe AssemblyVTableDescriptor* GetVtbl()
        {
            var vTable = new AssemblyVTable(AbiVersion)
            {
                InitializeDelegate = InitializeDelegate,
                DelegateLoggingDelegate = DelegateLoggingDelegate ?? ((d) => { return -1; }),
                BuildDelegate = BuildDelegate,
                GetHttpRequestHandlersDelegate = GetHttpRequestHandlersDelegate,
                GetHttpRequestHandlersMetadataDelegate = GetHttpRequestHandlersMetadataDelegate ?? (() => { return "EMPTY"; }),

                FreeAssemblyVTableDelegate = _freeAssemblyVTableDelegate,
                AssemblyDisposeDelegate = _assemblyDisposeDelegate
            };

            return AllocationUtilities.AllocCoTaskMem(AssemblyVTableDescriptor.Alloc(vTable));
        }

        private readonly unsafe FreeAssemblyVTableDelegate _freeAssemblyVTableDelegate = (desc) =>
        {
            if (desc == null)
                return 1;

            try
            {
                AllocationUtilities.FreeCoTaskMemIfNotNull(ref desc);
                return 0;
            }
            catch
            {
                return 2;
            }
        };

        private readonly unsafe AssemblyDisposeDelegate _assemblyDisposeDelegate = () =>
        {
            return 0;
        };
    }
}
