using Master.Application.Common;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetPagedJobs;

public class GetPagedJobPostsQuery : IRequest<PagedResult<JobPostResponseDTO>>
{
    public JobPostQuery Query { get; set; }

    public GetPagedJobPostsQuery(JobPostQuery query)
    {
        Query = query;
    }
}