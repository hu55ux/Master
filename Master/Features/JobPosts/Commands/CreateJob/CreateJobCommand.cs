using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Commands.CreateJob;

public record CreateJobCommand(Guid ClientId, CreateJobPostDTO Request) : IRequest<JobPostResponseDTO>;