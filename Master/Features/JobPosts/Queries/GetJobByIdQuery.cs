using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries;

public record GetJobByIdQuery(Guid Id) : IRequest<JobPostResponseDTO>;
