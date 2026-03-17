using AutoMapper;
using Master.DTOs;
using Master.Models;
using Master.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Features.Authorization.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponseDTO>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public RegisterUserHandler(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper, ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = _mapper.Map<AppUser>(request.Request);
        var result = await _userManager.CreateAsync(user, request.Request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        string roleToAssign = request.Request.Role == "Master" ? "Master" : "Customer";

        if (!await _roleManager.RoleExistsAsync(roleToAssign))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleToAssign));

        await _userManager.AddToRoleAsync(user, roleToAssign);

        return await _tokenService.GenerateTokensAsync(user);
    }
}
