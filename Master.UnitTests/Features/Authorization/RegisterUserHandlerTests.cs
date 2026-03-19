using AutoMapper;
using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.Authorization.Commands.RegisterUser;
using Master.Application.Interfaces;
using Master.Application.Models;
using Master.Application.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Master.UnitTests.Features.Authorization;

public class RegisterUserHandlerTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
        _mapperMock = new Mock<IMapper>();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new RegisterUserHandler(
            _authRepoMock.Object,
            _mapperMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAuthResponse_When_RegistrationIsSuccessful()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "huseyn@example.com",
            Password = "Password123!",
            Role = "Customer"
        };
        var command = new RegisterUserCommand(registerRequest);
        var user = new AppUser { Email = registerRequest.Email };
        var expectedResponse = new AuthResponseDTO { Email = user.Email, RefreshToken = "fake-jwt-token" };

        _authRepoMock.Setup(x => x.GetByEmailAsync(registerRequest.Email))
                     .ReturnsAsync((AppUser)null!);

        _mapperMock.Setup(m => m.Map<AppUser>(registerRequest))
                   .Returns(user);

        _authRepoMock.Setup(x => x.CreateUserAsync(user, registerRequest.Password))
                     .ReturnsAsync(IdentityResult.Success);

        _authRepoMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _authRepoMock.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).Returns(Task.CompletedTask);

        _tokenServiceMock.Setup(x => x.GenerateTokensAsync(user))
                         .ReturnsAsync(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be(registerRequest.Email);
        _tokenServiceMock.Verify(x => x.GenerateTokensAsync(user), Times.Once);
        _authRepoMock.Verify(x => x.AddToRoleAsync(user, "Customer"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserAlreadyExists()
    {
        var registerRequest = new RegisterRequest { Email = "exists@example.com" };
        var command = new RegisterUserCommand(registerRequest);

        _authRepoMock.Setup(x => x.GetByEmailAsync(registerRequest.Email))
                     .ReturnsAsync(new AppUser());

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("A user with this email already exists.");
    }
}