using System.Security.Claims;
using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.Authorization.Commands.RefreshToken;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Moq;

namespace Master.UnitTests.Features.Authorization;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new RefreshTokenHandler(_authRepositoryMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNewTokens_And_RevokeOldOne_When_TokenIsValid()
    {
        var refreshToken = "old-valid-refresh-token";
        var request = new RefreshTokenRequest { RefreshToken = refreshToken };
        var command = new RefreshTokenCommand(request);

        var jti = "old-jti-123";
        var userId = Guid.NewGuid().ToString();

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var storedToken = new RefreshToken
        {
            JwtId = jti,
            RevokedAt = null,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };
        var user = new AppUser { Id = Guid.Parse(userId) };
        var newTokens = new AuthResponseDTO { AccessToken = "new-access", RefreshToken = "new-Refresh" };
        var newJti = "new-jti-456";

        _tokenServiceMock.Setup(s => s.ValidateRefreshJwtAndGetJti(
                     It.IsAny<string>(),
                     It.IsAny<bool>()))
                  .Returns((principal, jti));

        _authRepositoryMock.Setup(r => r.GetRefreshTokenByJtiAsync(jti))
                     .ReturnsAsync(storedToken);

        _authRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
              .ReturnsAsync(user);

        _tokenServiceMock.Setup(s => s.GenerateTokensAsync(user))
                    .ReturnsAsync(newTokens);

        _tokenServiceMock.Setup(s => s.GetJtiFromRefreshToken(newTokens.RefreshToken))
                    .Returns(newJti);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access");

        storedToken.RevokedAt.Should().NotBeNull();
        storedToken.ReplacedByJwtId.Should().Be(newJti);

        _authRepositoryMock.Verify(r => r.UpdateRefreshToken(storedToken), Times.Once());
        _authRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_TokenIsInactive()
    {
        var request = new RefreshTokenRequest { RefreshToken = "revoked-token" };
        var command = new RefreshTokenCommand(request);
        var jti = "some-jti";

        _tokenServiceMock.Setup(s => s.ValidateRefreshJwtAndGetJti(
                     It.IsAny<string>(),
                     It.IsAny<bool>()))
                  .Returns((new ClaimsPrincipal(), jti));

        _authRepositoryMock.Setup(r => r.GetRefreshTokenByJtiAsync(jti))
                     .ReturnsAsync(new RefreshToken { ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1) });

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
          .WithMessage("Invalid token.");
    }
}
