using AutoMapper;
using Master.Data;
using Master.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.JobPosts.Queries;

public class GetAllJobsHandler : IRequestHandler<GetAllJobsQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetAllJobsHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetAllJobsQuery request, CancellationToken ct)
    {
        var jobs = await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}
