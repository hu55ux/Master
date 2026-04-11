using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Master.Application.Interfaces;
using MediatR;
using Master.Domain.Constants;

namespace Master.Application.Features.Authorization.Commands.RegisterUser;

/// <summary>
/// Handler for the <see cref="RegisterUserCommand"/>.
/// Responsible for user creation, role assignment, and token generation.
/// </summary>
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

    /// <summary>
    /// Handles the user registration process.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>An <see cref="AuthResponseDTO"/> containing tokens and user information.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user already exists or creation fails.</exception>
    public async Task<AuthResponseDTO> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = _mapper.Map<AppUser>(request.Request);
        var result = await _authRepository.CreateUserAsync(user, request.Request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        string roleToAssign = request.Request.Role == UserRoles.Master ? UserRoles.Master : UserRoles.Client;

        if (!await _authRepository.RoleExistsAsync(roleToAssign))
            await _authRepository.CreateRoleAsync(roleToAssign);

        await _authRepository.AddToRoleAsync(user, roleToAssign);

        return await _tokenService.GenerateTokensAsync(user);
    }
}