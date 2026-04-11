using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.CreateJob;

/// <summary>
/// Command for creating a new job post.
/// </summary>
/// <param name="ClientId">The ID of the customer creating the job.</param>
/// <param name="Request">The job details.</param>
public record CreateJobCommand(Guid ClientId, CreateJobPostDTO Request) : IRequest<JobPostResponseDTO>;