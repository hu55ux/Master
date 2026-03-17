using AutoMapper;
using Master.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.JobPosts.Commands.UpdateJob;

public class UpdateJobHandler : IRequestHandler<UpdateJobCommand, JobPostResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public UpdateJobHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(UpdateJobCommand command, CancellationToken ct)
    {
        var job = await _context.JobPosts
            .Include(j => j.Customer) 
            .Include(j => j.RequiredSkill)
            .FirstOrDefaultAsync(j => j.Id == command.JobId && j.CustomerId == command.ClientId, ct);

        if (job == null) throw new KeyNotFoundException($"Job not found: {command.JobId}");

        // Skill mövcudluğunu yoxlayırıq
        if (command.Request.RequiredSkillId.HasValue && command.Request.RequiredSkillId != Guid.Empty)
        {
            var skillExists = await _context.Skills.AnyAsync(s => s.Id == command.Request.RequiredSkillId.Value, ct);
            if (!skillExists) throw new KeyNotFoundException("Specified Skill ID does not exist.");
        }

        _mapper.Map(command.Request, job);

        await _context.SaveChangesAsync(ct);

        await _context.Entry(job).Reference(j => j.Customer).LoadAsync(ct);
        if (job.RequiredSkillId != Guid.Empty)
            await _context.Entry(job).Reference(j => j.RequiredSkill).LoadAsync(ct);

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}
