using AutoMapper;
using Master.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.JobPosts.Queries.GetMyJobs;

public class GetMyJobsHandler : IRequestHandler<GetMyJobsQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetMyJobsHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetMyJobsQuery request, CancellationToken ct)
    {
        var jobs = await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .Where(j => j.CustomerId == request.ClientId)
            .AsNoTracking()
            .ToListAsync(ct);

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}
