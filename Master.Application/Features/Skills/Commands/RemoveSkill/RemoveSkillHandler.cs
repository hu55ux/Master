using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Commands.RemoveSkill;

public class RemoveSkillHandler : IRequestHandler<RemoveSkillCommand, bool>
{
    private readonly MasterDbContext _context;

    public RemoveSkillHandler(MasterDbContext context) => _context = context;

    public async Task<bool> Handle(RemoveSkillCommand command, CancellationToken ct)
    {
        var userSkill = await _context.UserSkills
            .FirstOrDefaultAsync(x => x.UserId == command.MasterId && x.SkillId == command.SkillId, ct);

        if (userSkill == null) return false;

        _context.UserSkills.Remove(userSkill);
        var master = await _context.Users.FindAsync(new object[] { command.MasterId }, ct);
        if (master != null) master.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
