using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using System.Net;
using System.Text;

namespace bundle.Test.Plugin
{
    public class HandlerResultDescriptorBuilderExtensionsTests
    {
        [Fact]
        public void ToDescriptor_MapsStatusCodeDataAndContentType()
        {
            var result = HandlerResult
                .Create(HttpStatusCode.Created)
                .As(ContentType.ApplicationJson)
                .WithData("{\"id\":42}");

            var descriptor = result.ToDescriptor();

            try
            {
                Assert.Equal((int)HttpStatusCode.Created, descriptor.StatusCode);
                Assert.Equal((int)ContentType.ApplicationJson, descriptor.ContentType);
                Assert.Equal(
                    "{\"id\":42}",
                    AllocationUtilities.ToString(descriptor.Data, descriptor.DataLength));
                Assert.Equal(
                    Encoding.UTF8.GetByteCount("{\"id\":42}"),
                    descriptor.DataLength);
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public void ToDescriptor_MapsEmptyDataToNullPointer()
        {
            var result = HandlerResult.Create(HttpStatusCode.NoContent);

            var descriptor = result.ToDescriptor();

            try
            {
                Assert.Equal((int)HttpStatusCode.NoContent, descriptor.StatusCode);
                Assert.Equal((int)ContentType.TextPlain, descriptor.ContentType);
                Assert.Equal(nint.Zero, descriptor.Data);
                Assert.Equal(0, descriptor.DataLength);
            }
            finally
            {
                descriptor.Dispose();
            }
        }
    }
}
