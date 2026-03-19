using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Application.Models;
using Master.Application.Services;
using MediatR;


namespace Master.Application.Features.Authorization.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public RegisterUserHandler(IAuthRepository authRepository, IMapper mapper, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = _mapper.Map<AppUser>(request.Request);
        var result = await _authRepository.CreateUserAsync(user, request.Request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        string roleToAssign = request.Request.Role == "Master" ? "Master" : "Customer";

        if (!await _authRepository.RoleExistsAsync(roleToAssign))
            await _authRepository.CreateRoleAsync(roleToAssign);

        await _authRepository.AddToRoleAsync(user, roleToAssign);

        return await _tokenService.GenerateTokensAsync(user);
    }
}