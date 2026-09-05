using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.DeleteProfilePicture;

/// <summary>
/// Command to delete user's profile picture from Cloudinary CDN and clear ProfileImageUrl in database.
/// </summary>
public record DeleteProfilePictureCommand(Guid UserId) : IRequest<bool>;

/// <summary>
/// Handler for removing profile picture.
/// </summary>
public class DeleteProfilePictureHandler : IRequestHandler<DeleteProfilePictureCommand, bool>
{
    private readonly IAuthRepository _authRepository;
    private readonly IFileService _fileService;

    public DeleteProfilePictureHandler(IAuthRepository authRepository, IFileService fileService)
    {
        _authRepository = authRepository;
        _fileService = fileService;
    }

    public async Task<bool> Handle(DeleteProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            await _fileService.DeleteFileAsync(user.ProfileImageUrl);
            user.ProfileImageUrl = null;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _authRepository.UpdateAsync(user);
        }

        return true;
    }
}
