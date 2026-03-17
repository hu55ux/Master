using System.Security.Claims;
using Master.Common;
using Master.DTOs;
using Master.Features.Skills.Commands.AssignSkills;
using Master.Features.Skills.Commands.CreateSkill;
using Master.Features.Skills.Commands.RemoveSkill;
using Master.Features.Skills.Commands.UpdateSkill;
using Master.Features.Skills.Queries.GetAllSkills;
using Master.Features.Skills.Queries.GetById;
using Master.Features.Skills.Queries.GetMastersBySkill;
using Master.Features.Skills.Queries.GetMySkilss;
using Master.Features.Skills.Queries.GetPagedResult;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.Controllers;

/// <summary>
/// Controller for managing skills in the Master application. This controller provides endpoints for creating, updating, retrieving, and deleting skills,
/// as well as assigning skills to masters and retrieving masters by skill. It utilizes the MediatR library to handle commands and queries related to skills, 
/// allowing for a clean separation of concerns and promoting a maintainable architecture. Each endpoint is designed to perform specific operations related to skills
/// , such as creating a new skill, updating an existing skill, retrieving a paginated list of skills, and managing the association between skills and masters.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SkillController : ControllerBase
{

    /// <summary>
    /// Mediator instance for handling commands and queries related to skills. This allows the controller to delegate the processing of skill
    /// -related operations to the appropriate handlers, promoting a clean separation of concerns and enabling a more maintainable and scalable architecture.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// User ID of the currently authenticated user. This property retrieves the user's unique identifier from the claims in 
    /// the authentication token, allowing the controller to identify the user making requests and perform operations on their behalf when necessary.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Constructor for the SkillController. It initializes the controller with an instance of IMediator, 
    /// which is used to send commands and queries to the appropriate handlers for processing skill-related operations.
    /// </summary>
    /// <param name="mediator"></param>
    public SkillController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a list of all skills. This endpoint allows you to retrieve a complete list of all the skills available in the system.
    /// The response will include details about each skill, such as its name and description,
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllSkillsQuery());
        return Ok(ApiResponse<IEnumerable<SkillResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets a paginated list of skills. This endpoint allows you to retrieve a list of skills with pagination support. You can specify query parameters such as page number, page size, and optional filters to customize the results. The 
    /// response will include a paginated list of skills along with metadata about the total number of skills and pages available based on the provided pagination parameters.
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResult<SkillResponseDTO>>>> GetPaged([FromQuery] SkillQuery queryParams)
    {
        var result = await _mediator.Send(new GetPagedSkillsQuery(queryParams));
        return Ok(ApiResponse<PagedResult<SkillResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets a specific skill by its ID. This endpoint allows you to retrieve detailed information about a particular skill by providing its unique identifier in the URL. The response will include the skill's name, 
    /// description, and any other relevant details associated with that skill. If the skill with the specified ID does not exist, an appropriate error response will be returned.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetByIdQuery(id));
        return Ok(ApiResponse<SkillResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets a list of masters who possess a specific skill. This endpoint allows you to retrieve all the masters that have been assigned a particular skill by providing the skill ID in the URL. The response will include 
    /// details about each master, such as their name, contact information, and other relevant data, allowing you to easily identify which masters have the specified skill.
    /// </summary>
    [HttpGet("masters-by-skill/{skillId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuthResponseDTO>>>> GetMastersBySkill(Guid skillId)
    {
        var result = await _mediator.Send(new GetMastersBySkillQuery(skillId));
        return Ok(ApiResponse<IEnumerable<AuthResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Create a new skill. This endpoint allows you to add a new skill to the system by providing 
    /// the necessary information such as the skill name and description in the request body.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> Create([FromBody] CreateSkillDTO request)
    {
        var result = await _mediator.Send(new CreateSkillCommand(request));
        return Ok(ApiResponse<SkillResponseDTO>.SuccessResponse(result, "Skill created successfully."));
    }

    /// <summary>
    /// Updates an existing skill by its ID. This endpoint allows you to modify the details of a skill, such as its name and description.
    /// You need to provide the skill ID in the URL and the updated information in the request body.
    /// If the skill with the specified ID exists, it will be updated with the new information; otherwise, an appropriate error response will be returned.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<SkillResponseDTO>>> Update(Guid id, [FromBody] UpdateSkillDTO request)
    {
        var result = await _mediator.Send(new UpdateSkillCommand(id, request));
        return Ok(ApiResponse<SkillResponseDTO>.SuccessResponse(result, "Skill updated successfully."));
    }

    /// <summary>
    /// Assigns a list of skills to a master. This will add the specified skills to the master's existing skill 
    /// set without removing any previously assigned skills. If you want to replace the master's skills entirely, use the UpdateMasterSkills endpoint instead.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("assignMe")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignToMe([FromBody] List<Guid> skillIds)
    {
        var result = await _mediator.Send(new AssignSkillsToMasterCommand(UserId, skillIds));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Skills added to your profile."));
    }

    /// <summary>
    /// Removes a specific skill from a master. This will unassign the specified skill from the master without affecting 
    /// any other skills that the master may have. Use this endpoint when you want to remove a single skill from a master while keeping their other skills intact.
    /// </summary>
    [HttpDelete("removeSkill/{skillId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFromMe(Guid skillId)
    {
        var result = await _mediator.Send(new RemoveSkillCommand(UserId, skillId));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Skill removed from your profile."));
    }

    /// <summary>
    /// Gets a list of skills assigned to the currently authenticated master. This endpoint allows the master to retrieve all the skills that have been assigned to 
    /// their profile. The response will include details about each skill, such as its name and description, enabling the master to easily view and manage their skill set.
    /// </summary>
    /// <returns></returns>
    [HttpGet("my-skills")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<SkillResponseDTO>>>> GetMySkills()
    {
        var result = await _mediator.Send(new GetMySkillsQuery(UserId));
        return Ok(ApiResponse<IEnumerable<SkillResponseDTO>>.SuccessResponse(result));
    }
}

