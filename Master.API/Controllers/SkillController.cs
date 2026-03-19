using System.Security.Claims;
using Master.API.Extensions;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.Skills.Commands.AssignSkills;
using Master.Application.Features.Skills.Commands.CreateSkill;
using Master.Application.Features.Skills.Commands.RemoveSkill;
using Master.Application.Features.Skills.Commands.UpdateSkill;
using Master.Application.Features.Skills.Queries.GetAllSkills;
using Master.Application.Features.Skills.Queries.GetById;
using Master.Application.Features.Skills.Queries.GetMastersBySkill;
using Master.Application.Features.Skills.Queries.GetMySkilss;
using Master.Application.Features.Skills.Queries.GetPagedResult;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Controller for managing skills, including master-skill assignments and skill-based queries.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize] // All endpoints require authentication by default
public class SkillController : ControllerBase
{
    /// <summary>
    /// Mediator for handling requests and responses between the controller and application layer.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor for SkillController, injecting the IMediator instance for handling requests.
    /// </summary>
    /// <param name="mediator"></param>
    public SkillController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// UserId property retrieves the current authenticated user's ID from the claims. This is used for operations that require user context, such as assigning skills to the user's profile.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Retrieves all available skills. Accessible by everyone.
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllSkillsQuery());
        return Ok(ApiResponse<IEnumerable<SkillResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Retrieves a paged list of skills. Accessible by everyone.
    /// </summary>
    [HttpGet("paged")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedResult<SkillResponseDTO>>>> GetPaged([FromQuery] SkillQuery queryParams)
    {
        var result = await _mediator.Send(new GetPagedSkillsQuery(queryParams));
        return Ok(ApiResponse<PagedResult<SkillResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Creates a new skill. Restricted to Admin or specialized staff.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> Create([FromBody] CreateSkillDTO request)
    {
        var result = await _mediator.Send(new CreateSkillCommand(request));
        return Ok(ApiResponse<SkillResponseDTO>.SuccessResponse(result, "Skill created successfully."));
    }

    /// <summary>
    /// Updates an existing skill. Restricted to Admin.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> Update(Guid id, [FromBody] UpdateSkillDTO request)
    {
        var result = await _mediator.Send(new UpdateSkillCommand(id, request));
        return Ok(ApiResponse<SkillResponseDTO>.SuccessResponse(result, "Skill updated successfully."));
    }

    /// <summary>
    /// Assigns skills to the current user's profile. Requires a 'Master' role or valid profile.
    /// </summary>
    [HttpPost("assignMe")]
    [Authorize(Policy =AuthPolicies.MasterOrAdmin)]
    public async Task<ActionResult<ApiResponse<bool>>> AssignToMe([FromBody] List<Guid> skillIds)
    {
        var result = await _mediator.Send(new AssignSkillsToMasterCommand(UserId, skillIds));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Skills added to your profile."));
    }

    /// <summary>
    /// Removes a skill from the current user's profile.
    /// </summary>
    [HttpDelete("removeSkill/{skillId}")]
    [Authorize(Roles =AuthPolicies.MasterOrAdmin)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFromMe(Guid skillId)
    {
        var result = await _mediator.Send(new RemoveSkillCommand(UserId, skillId));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Skill removed from your profile."));
    }

    /// <summary>
    /// Gets skills for the authenticated user.
    /// </summary>
    [HttpGet("my-skills")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetMySkills()
    {
        var result = await _mediator.Send(new GetMySkillsQuery(UserId));
        return Ok(ApiResponse<IEnumerable<SkillResponseDTO>>.SuccessResponse(result));
    }
}