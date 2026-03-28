using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobStatuses;

public record GetJobStatusLookupQuery() : IRequest<List<JobStatusLookupDto>>;
