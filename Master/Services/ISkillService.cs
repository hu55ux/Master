namespace Master.Services;
using Master.DTOs;
using Master.Models;

public interface ISkillService
{
    Task<IEnumerable<SkillResponseDTO>> GetAllSkillsAsync();
    Task<SkillResponseDTO> GetSkillByIdAsync(Guid id);
    Task<Skill> GetSkillEntity(Guid id);
    Task<bool> AssignSkillsToMasterAsync(Guid masterId, List<Guid> skillIds);
    Task<IEnumerable<AuthResponseDTO>> GetMastersBySkillAsync(Guid skillId);
    Task<SkillResponseDTO> CreateSkillAsync(CreateSkillDTO request);
    Task<bool> RemoveSkillFromMasterAsync(Guid masterId, Guid skillId);
    Task<SkillResponseDTO> UpdateSkillAsync(Guid skillId, UpdateSkillDTO request);
    Task<bool> UpdateMasterSkillsAsync(Guid masterId, List<Guid> newSkillIds);
}
