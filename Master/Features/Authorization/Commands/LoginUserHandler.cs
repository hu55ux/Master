using Master.DTOs;
using Master.Models;
using Master.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Features.Authorization.Commands;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponseDTO>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await _tokenService.GenerateTokensAsync(user);
    }
}
