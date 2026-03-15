using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Commands;

public record CreateJobCommand(Guid ClientId, CreateJobPostDTO Request) : IRequest<JobPostResponseDTO>;