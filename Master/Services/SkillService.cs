using AutoMapper;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Services;

public class SkillService : ISkillService
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public SkillService(MasterDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    private IQueryable<AppUser> GetMastersWithSkills()
    {
        return _context.Users.Include(u => u.UserSkills);
    }

    private IQueryable<Skill> GetSkillsWithIncludes()
    {
        return _context.Skills.Include(s => s.UserSkills).ThenInclude(us => us.User);
    }

    public async Task<SkillResponseDTO> CreateSkillAsync(CreateSkillDTO request)
    {
        var nameExists = await _context.Skills
            .AnyAsync(s => s.Name.ToLower() == request.Name.ToLower());

        if (nameExists)
            throw new InvalidOperationException($"Skill '{request.Name}' already exists.");

        var newSkill = _mapper.Map<Skill>(request);
        _context.Skills.Add(newSkill);
        await _context.SaveChangesAsync();

        return _mapper.Map<SkillResponseDTO>(newSkill);
    }

    public async Task<IEnumerable<SkillResponseDTO>> GetAllSkillsAsync()
    {
        var skills = await _context.Skills.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<SkillResponseDTO>>(skills);
    }

    public async Task<SkillResponseDTO> GetSkillByIdAsync(Guid id)
    {
        var skill = await _context.Skills.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (skill is null) throw new KeyNotFoundException("Skill not found.");

        return _mapper.Map<SkillResponseDTO>(skill);
    }

    public async Task<IEnumerable<AuthResponseDTO>> GetMastersBySkillAsync(Guid skillId)
    {
        var masters = await _context.UserSkills
            .AsNoTracking()
            .Where(us => us.SkillId == skillId)
            .Include(us => us.User)
            .Select(us => us.User)
            .ToListAsync();

        return _mapper.Map<IEnumerable<AuthResponseDTO>>(masters);
    }

    public async Task<bool> AssignSkillsToMasterAsync(Guid masterId, List<Guid> skillIds)
    {
        var master = await _userManager.FindByIdAsync(masterId.ToString());
        if (master == null) throw new KeyNotFoundException("Master not found.");

        var validSkillIds = await _context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync();

        var existingSkillIds = await _context.UserSkills
            .Where(us => us.UserId == masterId)
            .Select(us => us.SkillId)
            .ToListAsync();

        var newSkills = validSkillIds
            .Where(id => !existingSkillIds.Contains(id))
            .Select(id => new UserSkill { UserId = masterId, SkillId = id })
            .ToList();

        if (newSkills.Any())
        {
            await _context.UserSkills.AddRangeAsync(newSkills);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> UpdateMasterSkillsAsync(Guid masterId, List<Guid> newSkillIds)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var master = await GetMastersWithSkills()
                .FirstOrDefaultAsync(u => u.Id == masterId);

            if (master is null) throw new KeyNotFoundException("Master not found.");

            _context.UserSkills.RemoveRange(master.UserSkills);

            var validIds = await _context.Skills
                .Where(s => newSkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var newUserSkills = validIds.Select(id => new UserSkill { UserId = masterId, SkillId = id }).ToList();

            await _context.UserSkills.AddRangeAsync(newUserSkills);
            master.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> RemoveSkillFromMasterAsync(Guid masterId, Guid skillId)
    {
        var master = await GetMastersWithSkills()
            .FirstOrDefaultAsync(u => u.Id == masterId);

        if (master is null) throw new KeyNotFoundException("Master not found.");

        var removingSkill = master.UserSkills.FirstOrDefault(s => s.SkillId == skillId);
        if (removingSkill is not null)
        {
            _context.UserSkills.Remove(removingSkill);
            master.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<SkillResponseDTO> UpdateSkillAsync(Guid skillId, UpdateSkillDTO request)
    {
        var skill = await _context.Skills.FindAsync(skillId);
        if (skill is null) throw new KeyNotFoundException("Skill not found.");

        _mapper.Map(request, skill);
        await _context.SaveChangesAsync();
        return _mapper.Map<SkillResponseDTO>(skill);
    }

    public async Task<Skill> GetSkillEntity(Guid id)
    {
        return await GetSkillsWithIncludes().FirstOrDefaultAsync(s => s.Id == id)
               ?? throw new KeyNotFoundException("Skill not found.");
    }
}