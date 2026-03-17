using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.CreateJob;

public record CreateJobCommand(Guid ClientId, CreateJobPostDTO Request) : IRequest<JobPostResponseDTO>;