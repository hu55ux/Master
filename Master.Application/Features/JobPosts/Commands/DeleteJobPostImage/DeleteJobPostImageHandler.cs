using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.DeleteJobPostImage;

/// <summary>
/// Command to delete a specific job post image attachment from Cloudinary CDN and database.
/// </summary>
public record DeleteJobPostImageCommand(Guid JobPostId, Guid ImageId, Guid CustomerId) : IRequest<bool>;

/// <summary>
/// Handler for removing a job post image attachment.
/// </summary>
public class DeleteJobPostImageHandler : IRequestHandler<DeleteJobPostImageCommand, bool>
{
    private readonly IJobPostRepository _jobPostRepository;
    private readonly IFileService _fileService;

    public DeleteJobPostImageHandler(IJobPostRepository jobPostRepository, IFileService fileService)
    {
        _jobPostRepository = jobPostRepository;
        _fileService = fileService;
    }

    public async Task<bool> Handle(DeleteJobPostImageCommand request, CancellationToken cancellationToken)
    {
        var jobPost = await _jobPostRepository.GetWithDetailsAsync(request.JobPostId, request.CustomerId, cancellationToken);
        if (jobPost == null)
        {
            throw new KeyNotFoundException("Job post not found or unauthorized.");
        }

        var image = jobPost.Images.FirstOrDefault(img => img.Id == request.ImageId);
        if (image == null)
        {
            throw new KeyNotFoundException("Job post image attachment not found.");
        }

        // Delete file from Cloudinary CDN
        await _fileService.DeleteFileAsync(image.ImageUrl);

        // Remove entity from job post collection
        jobPost.Images.Remove(image);

        _jobPostRepository.Update(jobPost);
        await _jobPostRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
