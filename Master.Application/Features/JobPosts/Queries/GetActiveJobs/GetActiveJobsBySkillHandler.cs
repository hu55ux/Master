using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Master.Application.Features.JobPosts.Queries.GetActiveJobs;

public class GetActiveJobsBySkillHandler : IRequestHandler<GetActiveJobsBySkillQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetActiveJobsBySkillHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetActiveJobsBySkillQuery request, CancellationToken ct)
    {
        var jobs = await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .Where(j => j.JPStatus == JobPostStatus.Active && j.RequiredSkillId == request.SkillId)
            .AsNoTracking()
            .ToListAsync(ct);

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}
