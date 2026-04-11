using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.Authorization.Commands.LoginUser;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Moq;
namespace Master.UnitTests.Features.Authorization;


public class LoginUserHandlerTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new LoginUserHandler(_authRepoMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAuthResponse_When_CredentialsAreValid()
    {
        var loginRequest = new LoginRequest { Email = "huseyn@example.com", Password = "CorrectPass123" };
        var command = new LoginUserCommand(loginRequest);
        var user = new AppUser { Email = loginRequest.Email };
        var expectedResponse = new AuthResponseDTO { Email = user.Email, AccessToken = "valid-jwt-token" };

        _authRepoMock.Setup(x => x.GetByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _authRepoMock.Setup(x => x.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);
        _tokenServiceMock.Setup(x => x.GenerateTokensAsync(user)).ReturnsAsync(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("valid-jwt-token");
    }

    [Theory]
    [InlineData("wrong@email.com", "anyPass", false, true)]
    [InlineData("huseyn@example.com", "WrongPass", true, false)]
    public async Task Handle_Should_ThrowUnauthorized_When_CredentialsAreInvalid(
        string email, string password, bool userExists, bool passwordCorrect)
    {
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var command = new LoginUserCommand(loginRequest);
        var user = userExists ? new AppUser { Email = email } : null;

        _authRepoMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user!);

        if (userExists)
        {
            _authRepoMock.Setup(x => x.CheckPasswordAsync(user!, password)).ReturnsAsync(passwordCorrect);
        }

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}