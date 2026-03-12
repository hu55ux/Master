using AutoMapper;
using Master.Common;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Services;

/// <summary>
/// Service responsible for managing skills and their assignment to masters.
/// </summary>
/// <remarks>
/// Handles creating, retrieving, updating, and deleting skills, as well as
/// assigning/removing skills for master users. Includes transaction handling
/// for bulk updates.
/// </remarks>
public class SkillService : ISkillService
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillService"/> class.
    /// </summary>
    /// <param name="context">Database context used for data access.</param>
    /// <param name="mapper">AutoMapper instance used for entity mapping.</param>
    /// <param name="userManager">Identity UserManager for master management.</param>
    public SkillService(MasterDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves a queryable collection of masters including their skills.
    /// </summary>
    /// <returns>A queryable collection of <see cref="AppUser"/> with their skills.</returns>
    private IQueryable<AppUser> GetMastersWithSkills()
        => _context.Users.Include(u => u.UserSkills);

    /// <summary>
    /// Retrieves a queryable collection of skills including assigned users.
    /// </summary>
    /// <returns>A queryable collection of <see cref="Skill"/> with related users.</returns>
    private IQueryable<Skill> GetSkillsWithIncludes()
        => _context.Skills.Include(s => s.UserSkills).ThenInclude(us => us.User);

    /// <summary>
    /// Creates a new skill in the system. Validates that a skill with the same name does not already exist before creation. 
    /// Maps the incoming DTO to a Skill entity, saves it to the database, and returns a SkillResponseDTO of the created skill.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Gets a list of all skills in the system. Maps the skill entities to a collection of SkillResponseDTOs for return.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SkillResponseDTO>> GetAllSkillsAsync()
    {
        var skills = await _context.Skills.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<SkillResponseDTO>>(skills);
    }

    /// <summary>
    /// Gets a skill by its ID. Throws a KeyNotFoundException if the skill does not exist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<SkillResponseDTO> GetSkillByIdAsync(Guid id)
    {
        var skill = await _context.Skills.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (skill == null) throw new KeyNotFoundException("Skill not found.");

        return _mapper.Map<SkillResponseDTO>(skill);
    }

    /// <summary>
    /// Gets a list of masters who have a specific skill by querying the UserSkills table and including related user data. Maps the result to a collection of AuthResponseDTOs.
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Assigns a list of skills to a master by creating associations in the UserSkills table.
    /// </summary>
    /// <param name="masterId"></param>
    /// <param name="skillIds"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
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

    /// <summary>
    /// Updates the skills assigned to a master by replacing existing associations with a new list of skill IDs.
    /// </summary>
    /// <param name="masterId"></param>
    /// <param name="newSkillIds"></param>
    /// <returns></returns>
    public async Task<bool> UpdateMasterSkillsAsync(Guid masterId, List<Guid> newSkillIds)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var master = await GetMastersWithSkills()
                .FirstOrDefaultAsync(u => u.Id == masterId);

            if (master == null) throw new KeyNotFoundException("Master not found.");

            _context.UserSkills.RemoveRange(master.UserSkills);

            var validIds = await _context.Skills
                .Where(s => newSkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var newUserSkills = validIds.Select(id => new UserSkill { UserId = masterId, SkillId = id }).ToList();

            await _context.UserSkills.AddRangeAsync(newUserSkills);
            master.UpdatedAt = DateTimeOffset.UtcNow;

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

    /// <summary>
    /// Removes a specific skill from a master. Validates that the master and skill association exists before removal.
    /// </summary>
    /// <param name="masterId"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<bool> RemoveSkillFromMasterAsync(Guid masterId, Guid skillId)
    {
        var userSkill = await _context.UserSkills
            .FirstOrDefaultAsync(x => x.UserId == masterId && x.SkillId == skillId);

        if (userSkill == null)
            return false;

        _context.UserSkills.Remove(userSkill);

        var master = await _context.Users.FindAsync(masterId);
        if (master != null)
            master.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Updates the details of an existing skill. Validates that the skill exists before applying updates.
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<SkillResponseDTO> UpdateSkillAsync(Guid skillId, UpdateSkillDTO request)
    {
        var skill = await _context.Skills.FindAsync(skillId);
        if (skill == null) throw new KeyNotFoundException("Skill not found.");

        _mapper.Map(request, skill);
        await _context.SaveChangesAsync();
        return _mapper.Map<SkillResponseDTO>(skill);
    }

    /// <summary>
    /// Gets a skill entity by its ID, including related user skills. Throws an exception if the skill is not found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<Skill> GetSkillEntity(Guid id)
    {
        return await GetSkillsWithIncludes().FirstOrDefaultAsync(s => s.Id == id)
               ?? throw new KeyNotFoundException("Skill not found.");
    }

    /// <summary>
    /// Gets paginated result.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<PagedResult<SkillResponseDTO>> GetPagedAsync(SkillQuery query)
    {
        query.Validate();

        var skillQuery = GetSkillsWithIncludes();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();

            skillQuery = skillQuery.Where(
                q => q.Name.ToLower().Contains(searchTerm) ||
                q.Description.ToLower().Contains(searchTerm)
                );
        }

        if (!string.IsNullOrEmpty(query.Sort))
        {
            skillQuery = ApplySorting(skillQuery, query.Sort, query.SortDirection);
        }

        else
        {
            skillQuery = skillQuery.OrderByDescending(c => c.Name);
        }
        var totalCount = await skillQuery.CountAsync();
        var skip = (query.Page - 1) * query.PageSize;
        var customers = await skillQuery
                                    .Skip(skip)
                                    .Take(query.PageSize)
                                    .ToListAsync();

        var skillDTO = _mapper.Map<IEnumerable<SkillResponseDTO>>(customers);

        return PagedResult<SkillResponseDTO>.Create(
            skillDTO,
            query.Page,
            query.PageSize,
            totalCount
            );

    }
    private IQueryable<Skill> ApplySorting(IQueryable<Skill> query, string sort, string sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";
        return sort.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "description" => isDescending ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
            _ => query.OrderByDescending(c => c.Name)
        };
    }
}