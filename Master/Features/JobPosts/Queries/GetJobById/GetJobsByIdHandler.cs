using AutoMapper;
using Master.Data;
using Master.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.JobPosts.Queries.GetJobById;

public class GetJobByIdHandler : IRequestHandler<GetJobByIdQuery, JobPostResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetJobByIdHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(GetJobByIdQuery request, CancellationToken ct)
    {
        var job = await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == request.Id, ct);

        if (job == null)
            throw new KeyNotFoundException($"Job with ID '{request.Id}' was not found.");

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}
