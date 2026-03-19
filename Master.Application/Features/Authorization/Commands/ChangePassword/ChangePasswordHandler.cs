using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Application.Services;
using MediatR;
namespace Master.Application.Features.Authorization.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public ChangePasswordHandler(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await _authRepository.GetByIdAsync(command.UserId.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var result = await _authRepository.ChangePasswordAsync(
            user,
            command.Request.CurrentPassword,
            command.Request.NewPassword
        );

        if (!result.Succeeded)
            throw new InvalidOperationException("Password change failed. Please check your current password.");

        return await _tokenService.GenerateTokensAsync(user);
    }
}