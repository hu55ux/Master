using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Application.Services;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.EditProfile;

public class EditProfileHandler : IRequestHandler<EditProfileCommand, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public EditProfileHandler(IAuthRepository authRepository, IMapper mapper, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(EditProfileCommand command, CancellationToken ct)
    {
        var user = await _authRepository.GetByIdAsync(command.UserId.ToString());
        if (user is null) throw new InvalidOperationException("User not found.");

        _mapper.Map(command.Request, user);

        var result = await _authRepository.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile updating failed: {errors}");
        }

        return await _tokenService.GenerateTokensAsync(user);
    }
}
