using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _authRepository.GetByEmailAsync(request.Request.Email);

        if (user is null || !await _authRepository.CheckPasswordAsync(user, request.Request.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await _tokenService.GenerateTokensAsync(user);
    }
}