using AutoMapper;
using Master.DTOs;
using Master.Models;
using Master.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Features.Authorization.Commands;

public class EditProfileHandler : IRequestHandler<EditProfileCommand, AuthResponseDTO>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public EditProfileHandler(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(EditProfileCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null) throw new InvalidOperationException("User not found.");

        _mapper.Map(command.Request, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile updating failed: {errors}");
        }

        return await _tokenService.GenerateTokensAsync(user);
    }
}
