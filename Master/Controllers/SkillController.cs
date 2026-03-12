using Master.Common;
using Master.DTOs;
using Master.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Master.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;
    private readonly IAuthService _authService;

    public SkillController(ISkillService skillService, IAuthService authService)
    {
        _skillService = skillService;
        _authService = authService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetAll()
    {
        var skills = await _skillService.GetAllSkillsAsync();

        return Ok(
            ApiResponse<IEnumerable<SkillResponseDTO>>
            .SuccessResponse(skills, "Skills retrieved successfully."));
    }


    /// <summary>
    /// Getting filtered results
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResult<SkillResponseDTO>>>> GetPaged([FromQuery] SkillQuery query)
    {
        var skills = await _skillService.GetPagedAsync(query);

        return Ok(ApiResponse<PagedResult<SkillResponseDTO>>.SuccessResponse(skills, "Jobs returned successfully."));
    }

    [HttpGet("{skillId}")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> GetSkillById(Guid skillId)
    {
        var skill = await _skillService.GetSkillByIdAsync(skillId);

        return Ok(
            ApiResponse<SkillResponseDTO>
            .SuccessResponse(skill, "Skill returned successfully."));
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> CreateSkill([FromBody] CreateSkillDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<Dictionary<string, string[]>>
                .ErrorResponse("Validation Error", ToValidationErrors(ModelState))
            );
        }

        var result = await _skillService.CreateSkillAsync(request);

        return Ok(
            ApiResponse<SkillResponseDTO>
            .SuccessResponse(result, "Skill created successfully"));
    }

    [HttpDelete("{skillId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSkill(Guid skillId)
    {
        var userId = _authService.UserId;

        var removed = await _skillService.RemoveSkillFromMasterAsync(userId, skillId);

        if (!removed)
            return NotFound(ApiResponse<object>.ErrorResponse("Skill not found."));

        return Ok(ApiResponse<object>.SuccessResponse("Skill deleted successfully"));
    }

    [HttpGet("mastersBySkill")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuthResponseDTO>>>> GetMastersBySkill(Guid skillId)
    {
        var masters = await _skillService.GetMastersBySkillAsync(skillId);

        return Ok(
            ApiResponse<IEnumerable<AuthResponseDTO>>
            .SuccessResponse(masters, "Masters retrieved successfully."));
    }

    [HttpPost("assignSkills")]
    public async Task<IActionResult> AssignSkillsToMaster([FromQuery] List<Guid> skillIds)
    {
        var userId = _authService.UserId;

        await _skillService.AssignSkillsToMasterAsync(userId, skillIds);

        return Ok(ApiResponse<object>.SuccessResponse("Skill assigning finished."));
    }

    [HttpPut("updateMasterSkills")]
    public async Task<IActionResult> UpdateMasterSkills([FromQuery] List<Guid> newSkillIds)
    {
        var userId = _authService.UserId;

        await _skillService.UpdateMasterSkillsAsync(userId, newSkillIds);

        return Ok(ApiResponse<object>.SuccessResponse("Skill updating finished."));
    }

    private Dictionary<string, string[]> ToValidationErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }
}