using Master.Domain.Models;

namespace Master.Application.Interfaces;

public interface ISkillRepository
{
    /// <summary>
    /// Checks if a master (user) exists in the database.
    /// </summary>
    Task<bool> MasterExistsAsync(Guid masterId, CancellationToken ct);

    /// <summary>
    /// Filters and returns only the IDs that exist in the Skills table from a given list.
    /// </summary>
    Task<List<Guid>> GetValidSkillIdsAsync(List<Guid> skillIds, CancellationToken ct);

    /// <summary>
    /// Retrieves the IDs of skills already assigned to a specific master.
    /// </summary>
    Task<List<Guid>> GetExistingUserSkillIdsAsync(Guid masterId, CancellationToken ct);

    /// <summary>
    /// Adds a range of new user-skill associations.
    /// </summary>
    Task AddUserSkillsRangeAsync(IEnumerable<UserSkill> userSkills, CancellationToken ct);

    /// <summary>
    /// Updates the timestamp of the user profile.
    /// </summary>
    Task UpdateUserTimestampAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Persists all changes to the database.
    /// </summary>
    Task<bool> SaveChangesAsync(CancellationToken ct);

    /// <summary>
    /// Checks if a skill with the specified name already exists (case-insensitive).
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);

    /// <summary>
    /// Adds a new skill entity to the database.
    /// </summary>
    Task AddAsync(Skill skill, CancellationToken ct);
    /// <summary>
    /// Retrieves a specific user-skill association based on user and skill identifiers.
    /// </summary>
    Task<UserSkill?> GetUserSkillAsync(Guid userId, Guid skillId, CancellationToken ct);

    /// <summary>
    /// Removes a user-skill association from the database.
    /// </summary>
    void RemoveUserSkill(UserSkill userSkill);

    /// <summary>
    /// Retrieves a skill by its unique identifier.
    /// </summary>
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Updates an existing skill entity in the database.
    /// </summary>
    void Update(Skill skill);

    /// <summary>
    /// Retrieves all available skills from the database without tracking changes.
    /// </summary>
    Task<IEnumerable<Skill>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves a collection of master users associated with a specific skill identifier.
    /// </summary>
    /// <param name="skillId">The unique identifier of the skill.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of users who possess the specified skill.</returns>
    Task<IEnumerable<AppUser>> GetMastersBySkillAsync(Guid skillId, CancellationToken ct);

    /// <summary>
    /// Retrieves a collection of skills associated with a specific user identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user (master).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of skills belonging to the specified user.</returns>
    Task<IEnumerable<Skill>> GetSkillsByUserIdAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Retrieves a paged, filtered, and sorted list of skills.
    /// </summary>
    /// <param name="query">The query parameters containing page, size, search term, and sort details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple containing the list of skills and the total record count.</returns>
    Task<(IEnumerable<Skill> Items, int TotalCount)> GetPagedAsync(SkillQuery query, CancellationToken ct);
}
