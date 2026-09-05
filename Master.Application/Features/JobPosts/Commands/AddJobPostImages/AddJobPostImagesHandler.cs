using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Master.Application.Features.JobPosts.Commands.AddJobPostImages;

/// <summary>
/// Command to upload multiple job photo attachments to Cloudinary for a specific job post.
/// </summary>
public record AddJobPostImagesCommand(Guid JobPostId, Guid CustomerId, List<IFormFile> Files) : IRequest<List<string>>;

/// <summary>
/// Handler for uploading job post images to Cloudinary and persisting references in database.
/// </summary>
public class AddJobPostImagesHandler : IRequestHandler<AddJobPostImagesCommand, List<string>>
{
    private readonly IJobPostRepository _jobPostRepository;
    private readonly IFileService _fileService;

    public AddJobPostImagesHandler(IJobPostRepository jobPostRepository, IFileService fileService)
    {
        _jobPostRepository = jobPostRepository;
        _fileService = fileService;
    }

    public async Task<List<string>> Handle(AddJobPostImagesCommand request, CancellationToken cancellationToken)
    {
        if (request.Files == null || request.Files.Count == 0)
        {
            throw new ArgumentException("At least one image file must be provided.");
        }

        var jobPost = await _jobPostRepository.GetWithDetailsAsync(request.JobPostId, request.CustomerId, cancellationToken);
        if (jobPost == null)
        {
            throw new KeyNotFoundException("Job post not found or you are not authorized.");
        }

        var uploadedUrls = new List<string>();

        foreach (var file in request.Files)
        {
            if (file.Length == 0) continue;

            var secureUrl = await _fileService.UploadFileAsync(file, "job-posts");
            jobPost.Images.Add(new JobPostImage
            {
                Id = Guid.NewGuid(),
                JobPostId = jobPost.Id,
                ImageUrl = secureUrl,
                UploadedAt = DateTimeOffset.UtcNow
            });
            uploadedUrls.Add(secureUrl);
        }

        _jobPostRepository.Update(jobPost);
        await _jobPostRepository.SaveChangesAsync(cancellationToken);
        return uploadedUrls;
    }
}
