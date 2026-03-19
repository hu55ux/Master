using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.UpdateJob;

public class UpdateJobHandler : IRequestHandler<UpdateJobCommand, JobPostResponseDTO>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public UpdateJobHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(UpdateJobCommand command, CancellationToken ct)
    {
        var job = await _jobRepository.GetWithDetailsAsync(command.JobId, command.ClientId, ct);
        if (job == null) throw new KeyNotFoundException($"Job not found: {command.JobId}");

        if (command.Request.RequiredSkillId.HasValue && command.Request.RequiredSkillId != Guid.Empty)
        {
            var skillExists = await _jobRepository.SkillExistsAsync(command.Request.RequiredSkillId.Value, ct);
            if (!skillExists) throw new KeyNotFoundException("Specified Skill ID does not exist.");
        }

        _mapper.Map(command.Request, job);
        _jobRepository.Update(job);
        await _jobRepository.SaveChangesAsync(ct);

        await _jobRepository.LoadReferencesAsync(job, ct);

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}