namespace Master.Services;

using Master.Common;
using Master.DTOs;
using Master.Models;

/// <summary>
/// Defines operations related to skill management in the system.
/// </summary>
/// <remarks>
/// This service handles creation, retrieval, updating, and assignment
/// of skills to masters (workers).
/// </remarks>
public interface ISkillService
{
    /// <summary>
    /// Retrieves all skills available in the system.
    /// </summary>
    /// <returns>A collection of skill response DTOs.</returns>
    Task<IEnumerable<SkillResponseDTO>> GetAllSkillsAsync();

    /// <summary>
    /// Gets filtered result
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<SkillResponseDTO>> GetPagedAsync(SkillQuery query);

    /// <summary>
    /// Retrieves a specific skill by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the skill.</param>
    /// <returns>The skill information.</returns>
    Task<SkillResponseDTO> GetSkillByIdAsync(Guid id);

    /// <summary>
    /// Retrieves the skill entity directly from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the skill.</param>
    /// <returns>The skill entity.</returns>
    /// <remarks>
    /// This method returns the entity instead of a DTO and is usually
    /// used internally for business logic operations.
    /// </remarks>
    Task<Skill> GetSkillEntity(Guid id);

    /// <summary>
    /// Assigns multiple skills to a master (worker).
    /// </summary>
    /// <param name="masterId">The identifier of the master.</param>
    /// <param name="skillIds">The list of skill identifiers to assign.</param>
    /// <returns>True if the assignment was successful; otherwise false.</returns>
    Task<bool> AssignSkillsToMasterAsync(Guid masterId, List<Guid> skillIds);

    /// <summary>
    /// Retrieves all masters who have a specific skill.
    /// </summary>
    /// <param name="skillId">The identifier of the skill.</param>
    /// <returns>A collection of masters with the specified skill.</returns>
    Task<IEnumerable<AuthResponseDTO>> GetMastersBySkillAsync(Guid skillId);

    /// <summary>
    /// Creates a new skill in the system.
    /// </summary>
    /// <param name="request">The skill creation data.</param>
    /// <returns>The created skill.</returns>
    Task<SkillResponseDTO> CreateSkillAsync(CreateSkillDTO request);

    /// <summary>
    /// Removes a specific skill from a master.
    /// </summary>
    /// <param name="masterId">The identifier of the master.</param>
    /// <param name="skillId">The identifier of the skill to remove.</param>
    /// <returns>True if the skill was successfully removed; otherwise false.</returns>
    Task<bool> RemoveSkillFromMasterAsync(Guid masterId, Guid skillId);

    /// <summary>
    /// Updates an existing skill.
    /// </summary>
    /// <param name="skillId">The identifier of the skill to update.</param>
    /// <param name="request">The updated skill data.</param>
    /// <returns>The updated skill.</returns>
    Task<SkillResponseDTO> UpdateSkillAsync(Guid skillId, UpdateSkillDTO request);

    /// <summary>
    /// Updates all skills assigned to a master.
    /// </summary>
    /// <param name="masterId">The identifier of the master.</param>
    /// <param name="newSkillIds">The new list of skill identifiers.</param>
    /// <returns>True if the update operation was successful; otherwise false.</returns>
    Task<bool> UpdateMasterSkillsAsync(Guid masterId, List<Guid> newSkillIds);
}