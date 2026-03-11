using Master.Common;
using Master.DTOs;
using Master.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration.UserSecrets;
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

    [HttpGet("All")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetAll()
    {
        var skills = await _skillService.GetAllSkillsAsync();

        if (skills is null || !skills.Any())
        {
            return NotFound(
                ApiResponse<IEnumerable<JobPostResponseDTO>>
                .ErrorResponse("Skills not found!")
            );
        }

        return Ok(
            ApiResponse<IEnumerable<SkillResponseDTO>>
            .SuccessResponse(skills, "Skills retrieved successfully."));
    }

    [HttpGet("byID/{skillId}")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> GetSkillById(Guid skillId)
    {
        var skill = await _skillService.GetSkillByIdAsync(skillId);

        if (skill is null)
        {
            return NotFound(
                ApiResponse<object>
                .ErrorResponse("Skill not found by your Id"));
        }

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

        if (result == null)
        {
            return BadRequest(
                ApiResponse<SkillResponseDTO>
                .ErrorResponse("Skill could not be created")
            );
        }

        return Ok(
            ApiResponse<SkillResponseDTO>
            .SuccessResponse(result, "skill created successfully")
        );
    }

    [HttpDelete("remove/{skillId}")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>> DeleteSkill(Guid skillId)
    {
        var userId = _authService.UserId;

        var skill = await _skillService.RemoveSkillFromMasterAsync(userId, skillId);
    }























    /// <summary>
    /// Converts ModelState validation errors into a dictionary format.
    /// </summary>
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
