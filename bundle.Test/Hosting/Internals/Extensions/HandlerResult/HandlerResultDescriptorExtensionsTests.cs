using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;
using System.Net;

namespace bundle.Test.Hosting
{
    public class HandlerResultDescriptorExtensionsTests
    {
        [Fact]
        public void FromDescriptor_MapsStatusCodeDataAndContentType()
        {
            var data = AllocationUtilities.AllocString("payload", out var dataLength);

            var descriptor = new HandlerResultDescriptor(
                (int)HttpStatusCode.Accepted,
                data,
                dataLength,
                (int)ContentType.TextHtml);

            var result = HandlerResult.FromDescriptor(descriptor);

            Assert.Equal((int)HttpStatusCode.Accepted, result.StatusCode);
            Assert.Equal(ContentType.TextHtml, result.ContentType);
            Assert.Equal("payload", result.Data);
        }

        [Fact]
        public void FromDescriptor_MapsEmptyData()
        {
            var descriptor = new HandlerResultDescriptor(
                (int)HttpStatusCode.NoContent,
                nint.Zero,
                0,
                (int)ContentType.TextPlain);

            var result = HandlerResult.FromDescriptor(descriptor);

            Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
            Assert.Equal(ContentType.TextPlain, result.ContentType);
            Assert.Equal(string.Empty, result.Data);
        }
    }
}
