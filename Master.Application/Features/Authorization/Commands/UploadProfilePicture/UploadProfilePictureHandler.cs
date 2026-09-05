using Master.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Master.Application.Features.Authorization.Commands.UploadProfilePicture;

/// <summary>
/// Command to upload or update user profile picture on Cloudinary CDN.
/// </summary>
public record UploadProfilePictureCommand(Guid UserId, IFormFile File) : IRequest<string>;

/// <summary>
/// Handler for uploading profile picture and updating user's ProfileImageUrl property.
/// </summary>
public class UploadProfilePictureHandler : IRequestHandler<UploadProfilePictureCommand, string>
{
    private readonly IAuthRepository _authRepository;
    private readonly IFileService _fileService;

    public UploadProfilePictureHandler(IAuthRepository authRepository, IFileService fileService)
    {
        _authRepository = authRepository;
        _fileService = fileService;
    }

    public async Task<string> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            throw new ArgumentException("Profile picture file cannot be empty.");
        }

        var user = await _authRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        // Delete old profile picture if exists on Cloudinary
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            await _fileService.DeleteFileAsync(user.ProfileImageUrl);
        }

        // Upload new picture to Cloudinary CDN
        var uploadedUrl = await _fileService.UploadFileAsync(request.File, "profile-pictures");

        user.ProfileImageUrl = uploadedUrl;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _authRepository.UpdateAsync(user);

        return uploadedUrl;
    }
}
