using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobById;

public record GetJobByIdQuery(Guid Id) : IRequest<JobPostResponseDTO>;
