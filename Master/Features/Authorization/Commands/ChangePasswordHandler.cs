using Master.DTOs;
using Master.Models;
using Master.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Features.Authorization.Commands;
public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, AuthResponseDTO>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public ChangePasswordHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null) throw new InvalidOperationException("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, command.Request.CurrentPassword, command.Request.NewPassword);
        if (!result.Succeeded) throw new InvalidOperationException("Password change failed.");

        return await _tokenService.GenerateTokensAsync(user);
    }
}