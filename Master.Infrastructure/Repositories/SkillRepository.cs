using Master.Application.Interfaces;
using Master.Application.Models;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly MasterDbContext _context;

    public SkillRepository(MasterDbContext context) => _context = context;

    public async Task<bool> MasterExistsAsync(Guid masterId, CancellationToken ct)
        => await _context.Users.AnyAsync(u => u.Id == masterId, ct);

    public async Task<List<Guid>> GetValidSkillIdsAsync(List<Guid> skillIds, CancellationToken ct)
        => await _context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .Select(s => s.Id).ToListAsync(ct);

    public async Task<List<Guid>> GetExistingUserSkillIdsAsync(Guid masterId, CancellationToken ct)
        => await _context.UserSkills
            .Where(us => us.UserId == masterId)
            .Select(us => us.SkillId).ToListAsync(ct);

    public async Task AddUserSkillsRangeAsync(IEnumerable<UserSkill> userSkills, CancellationToken ct)
     => await _context.UserSkills.AddRangeAsync(userSkills, ct);

    public async Task UpdateUserTimestampAsync(Guid userId, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, ct);
        if (user != null) user.UpdatedAt = DateTimeOffset.UtcNow;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct) > 0;

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        return await _context.Skills
            .AnyAsync(s => s.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task AddAsync(Skill skill, CancellationToken ct)
    {
        await _context.Skills.AddAsync(skill, ct);
    }

    public async Task<UserSkill?> GetUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        return await _context.UserSkills
            .FirstOrDefaultAsync(x => x.UserId == userId && x.SkillId == skillId, ct);
    }

    public void RemoveUserSkill(UserSkill userSkill)
    {
        _context.UserSkills.Remove(userSkill);
    }

    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Skills.FindAsync(new object[] { id }, ct);
    }

    public void Update(Skill skill)
    {
        _context.Skills.Update(skill);
    }

    public async Task<IEnumerable<Skill>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Skills
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AppUser?>> GetMastersBySkillAsync(Guid skillId, CancellationToken ct)
    {
        return await _context.UserSkills
            .AsNoTracking()
            .Where(us => us.SkillId == skillId)
            .Include(us => us.User)
            .Select(us => us.User)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Skill?>> GetSkillsByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _context.UserSkills
            .AsNoTracking()
            .Where(us => us.UserId == userId)
            .Include(us => us.Skill)
            .Select(us => us.Skill)
            .ToListAsync(ct);
    }
    public async Task<(IEnumerable<Skill> Items, int TotalCount)> GetPagedAsync(SkillQuery query, CancellationToken ct)
    {
        var skillQuery = _context.Skills
            .Include(s => s.UserSkills)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLower();
            skillQuery = skillQuery.Where(s =>
                s.Name.ToLower().Contains(term) ||
                s.Description.ToLower().Contains(term));
        }

        skillQuery = ApplySorting(skillQuery, query.Sort, query.SortDirection);

        var totalCount = await skillQuery.CountAsync(ct);

        var skip = (query.Page - 1) * query.PageSize;
        var items = await skillQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    /// <summary>
    /// Helper method to apply dynamic sorting to the skill query.
    /// </summary>
    private IQueryable<Skill> ApplySorting(IQueryable<Skill> query, string sort, string direction)
    {
        var isDesc = direction?.ToLower() == "desc";

        return sort?.ToLower() switch
        {
            "title" => isDesc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "description" => isDesc ? query.OrderByDescending(s => s.Description) : query.OrderBy(s => s.Description),
            _ => query.OrderByDescending(s => s.Name)
        };
    }
}
