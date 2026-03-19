namespace Master.Application.Features.Authorization.Commands.DeleteProfile;

using Master.Application.Interfaces;
using MediatR;

public class DeleteProfileHandler : IRequestHandler<DeleteProfileCommand, bool>
{
    private readonly IAuthRepository _authRepository;

    public DeleteProfileHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<bool> Handle(DeleteProfileCommand command, CancellationToken ct)
    {
        var user = await _authRepository.GetUserWithDetailsAsync(command.UserId, ct);

        if (user is null)
            throw new KeyNotFoundException("Profile not found.");

        var result = await _authRepository.FullDeleteUserAsync(user, ct);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User deleting failed: {errors}");
        }

        return true;
    }
}