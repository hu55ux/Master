using Master.DTOs;

namespace Master.Services;

public class AuthService : IAuthService
{
    public Task<AuthResponseDTO> ChangePasswordAsync(Guid id, ChangePasswordRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDTO> EditOwnProfileAsync(Guid id, ProfileEditRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDTO> LoginUserAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDTO?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDTO> RegisterUserAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
