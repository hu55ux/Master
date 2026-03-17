using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.UpdateJob;

public record UpdateJobCommand(Guid JobId, Guid ClientId, UpdateJobPostDTO Request) : IRequest<JobPostResponseDTO>;
