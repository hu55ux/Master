using System.IO;
using FluentAssertions;
using Master.Infrastructure.Config;
using Master.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace Master.UnitTests.Features.Notifications;

public class CloudinaryLiveUploadTest
{
    [Fact]
    public async Task Live_Cloudinary_Upload_Should_Return_Valid_SecureUrl()
    {
        var config = Options.Create(new CloudinaryConfig
        {
            CloudName = "dazyrrix",
            ApiKey = "915869718866468",
            ApiSecret = "4Kw9DtBNzd4KWNvCRwjLTr3XWFs"
        });

        var cloudinaryService = new CloudinaryService(config);

        // 1x1 pixel PNG dummy image byte stream
        byte[] dummyPngBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");
        using var stream = new MemoryStream(dummyPngBytes);
        var file = new FormFile(stream, 0, dummyPngBytes.Length, "file", "master_live_test_image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var uploadedUrl = await cloudinaryService.UploadFileAsync(file, "chat-media-test");

        uploadedUrl.Should().NotBeNullOrEmpty();
        uploadedUrl.Should().StartWith("https://res.cloudinary.com/dazyrrix/");
    }
}
