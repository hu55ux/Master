using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Commands.AssignSkills;

public class AssignSkillsToMasterHandler : IRequestHandler<AssignSkillsToMasterCommand, bool>
{
    private readonly ISkillRepository _skillRepository;

    public AssignSkillsToMasterHandler(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<bool> Handle(AssignSkillsToMasterCommand command, CancellationToken ct)
    {
        if (!await _skillRepository.MasterExistsAsync(command.MasterId, ct))
            throw new KeyNotFoundException("Master profile not found.");

        var validSkillIds = await _skillRepository.GetValidSkillIdsAsync(command.SkillIds, ct);

        var existingSkillIds = await _skillRepository.GetExistingUserSkillIdsAsync(command.MasterId, ct);

        var newSkills = validSkillIds
            .Where(id => !existingSkillIds.Contains(id))
            .Select(id => new UserSkill { UserId = command.MasterId, SkillId = id })
            .ToList();

        if (newSkills.Any())
        {
            await _skillRepository.AddUserSkillsRangeAsync(newSkills, ct);
            await _skillRepository.UpdateUserTimestampAsync(command.MasterId, ct);

            await _skillRepository.SaveChangesAsync(ct);
        }

        return true;
    }
}

