using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.Authorization.Commands.RegisterDeviceToken;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Moq;
using Xunit;

namespace Master.UnitTests.Features.Notifications;

public class NotificationTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;

    public NotificationTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
    }

    [Fact]
    public async Task RegisterDeviceTokenHandler_Should_UpdateUserDeviceToken_Successfully()
    {
        var userId = Guid.NewGuid();
        var user = new AppUser { Id = userId, Email = "test@example.com" };
        var request = new RegisterDeviceTokenRequest
        {
            DeviceToken = "fcm_token_123456789",
            DeviceType = "android"
        };
        var command = new RegisterDeviceTokenCommand(userId, request);

        _authRepoMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        _authRepoMock.Setup(x => x.UpdateAsync(user))
                     .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

        var handler = new RegisterDeviceTokenHandler(_authRepoMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        user.DeviceToken.Should().Be("fcm_token_123456789");
        user.DeviceType.Should().Be("android");
        _authRepoMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task RegisterDeviceTokenHandler_Should_ThrowKeyNotFoundException_When_UserNotFound()
    {
        var userId = Guid.NewGuid();
        var request = new RegisterDeviceTokenRequest { DeviceToken = "token" };
        var command = new RegisterDeviceTokenCommand(userId, request);

        _authRepoMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((AppUser)null!);

        var handler = new RegisterDeviceTokenHandler(_authRepoMock.Object);
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("User not found.");
    }
}
