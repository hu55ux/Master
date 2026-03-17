using Master.Data;
using Master.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.Skills.Commands.AssignSkills;

public class AssignSkillsToMasterHandler : IRequestHandler<AssignSkillsToMasterCommand, bool>
{
    private readonly MasterDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AssignSkillsToMasterHandler(MasterDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> Handle(AssignSkillsToMasterCommand command, CancellationToken ct)
    {
        var masterExists = await _context.Users.AnyAsync(u => u.Id == command.MasterId, ct);
        if (!masterExists) throw new KeyNotFoundException("Master profile not found.");

        var validSkillIds = await _context.Skills
            .Where(s => command.SkillIds.Contains(s.Id))
            .Select(s => s.Id).ToListAsync(ct);

        var existingSkillIds = await _context.UserSkills
            .Where(us => us.UserId == command.MasterId)
            .Select(us => us.SkillId).ToListAsync(ct);

        var newSkills = validSkillIds
            .Where(id => !existingSkillIds.Contains(id))
            .Select(id => new UserSkill { UserId = command.MasterId, SkillId = id }).ToList();

        if (newSkills.Any())
        {
            await _context.UserSkills.AddRangeAsync(newSkills, ct);

            var master = await _context.Users.FindAsync(command.MasterId, ct);
            if (master != null) master.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        return true;
    }
}

