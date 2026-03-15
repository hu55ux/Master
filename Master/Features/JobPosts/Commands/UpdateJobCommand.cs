using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Commands;

public record UpdateJobCommand(Guid JobId, Guid ClientId, UpdateJobPostDTO Request) : IRequest<JobPostResponseDTO>;
