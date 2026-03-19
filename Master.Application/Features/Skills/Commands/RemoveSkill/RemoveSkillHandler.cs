using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Commands.RemoveSkill;

public class RemoveSkillHandler : IRequestHandler<RemoveSkillCommand, bool>
{
    private readonly ISkillRepository _skillRepository;

    public RemoveSkillHandler(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<bool> Handle(RemoveSkillCommand command, CancellationToken ct)
    {
        var userSkill = await _skillRepository.GetUserSkillAsync(command.MasterId, command.SkillId, ct);

        if (userSkill == null) return false;

        _skillRepository.RemoveUserSkill(userSkill);

        await _skillRepository.UpdateUserTimestampAsync(command.MasterId, ct);

        return await _skillRepository.SaveChangesAsync(ct);
    }
}
