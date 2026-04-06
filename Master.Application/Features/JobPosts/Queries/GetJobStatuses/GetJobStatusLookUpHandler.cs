using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobStatuses;

public class GetJobStatusLookupHandler : IRequestHandler<GetJobStatusLookupQuery, List<JobStatusLookupDto>>
{
    public Task<List<JobStatusLookupDto>> Handle(GetJobStatusLookupQuery request, CancellationToken ct)
    {
        var statuses = Enum.GetValues(typeof(JobPostStatus))
            .Cast<JobPostStatus>()
            .Select(s => new JobStatusLookupDto((int)s, s.ToString()))
            .ToList();

        return Task.FromResult(statuses);
    }
}
