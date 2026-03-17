using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.CreateJob;

public class CreateJobHandler : IRequestHandler<CreateJobCommand, JobPostResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public CreateJobHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(CreateJobCommand command, CancellationToken ct)
    {
        var job = _mapper.Map<JobPost>(command.Request);
        job.CustomerId = command.ClientId;
        job.JPStatus = JobPostStatus.Active;

        _context.JobPosts.Add(job);
        await _context.SaveChangesAsync(ct);

        await _context.Entry(job).Reference(j => j.Customer).LoadAsync(ct);
        if (job.RequiredSkillId != Guid.Empty)
            await _context.Entry(job).Reference(j => j.RequiredSkill).LoadAsync(ct);

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}
