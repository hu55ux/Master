using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries.GetJobById;

public record GetJobByIdQuery(Guid Id) : IRequest<JobPostResponseDTO>;
