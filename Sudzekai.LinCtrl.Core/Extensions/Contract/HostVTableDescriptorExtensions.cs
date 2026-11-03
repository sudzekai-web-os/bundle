
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class HostVTableDescriptorExtensions
    {
        extension(HostVTableDescriptor descriptor)
        {
            public static HostVTableDescriptor Alloc(
                uint abiVersion,
                FreeHttpRequestDelegateDescriptor freeHttpRequestDelegate)
            {
                FreeHttpRequestDelegateDescriptor* freeHttpRequestDelegatePointer = null;

                try
                {
                    freeHttpRequestDelegatePointer = AllocationUtilities.AllocCoTaskMem(freeHttpRequestDelegate);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref freeHttpRequestDelegatePointer);

                    throw;
                }

                return new()
                {
                    AbiVersion = abiVersion,
                    FreeHttpRequestDelegate = freeHttpRequestDelegatePointer
                };
            }

            public static HostVTableDescriptor Alloc(HostVTable source)
            {
                ArgumentNullException.ThrowIfNull(source.FreeHttpRequestDelegate, nameof(source.FreeHttpRequestDelegate));

                FreeHttpRequestDelegateDescriptor freeHttpRequestDelegateDescriptor = default;

                try
                {
                    freeHttpRequestDelegateDescriptor = FreeHttpRequestDelegateDescriptor.Alloc(source.FreeHttpRequestDelegate);

                    return Alloc(source.AbiVersion, freeHttpRequestDelegateDescriptor);
                }
                catch
                {
                    freeHttpRequestDelegateDescriptor.Free();

                    throw;
                }
            }

            public HostVTable ToManaged()
            {
                var freeHttpRequestDelegate = descriptor.FreeHttpRequestDelegate == null
                    ? null : descriptor.FreeHttpRequestDelegate->ToManaged();

                return new(descriptor.AbiVersion)
                {
                    FreeHttpRequestDelegate = freeHttpRequestDelegate
                };
            }
        }
    }
}