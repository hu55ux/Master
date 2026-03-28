using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.JobPosts.Commands.ChangejobStatus;
using Master.Application.Features.JobPosts.Commands.CreateJob;
using Master.Application.Features.JobPosts.Commands.UpdateJob;
using Master.Application.Features.JobPosts.Queries.GetActiveJobs;
using Master.Application.Features.JobPosts.Queries.GetAllJobs;
using Master.Application.Features.JobPosts.Queries.GetJobById;
using Master.Application.Features.JobPosts.Queries.GetJobsByUser;
using Master.Application.Features.JobPosts.Queries.GetJobStatuses;
using Master.Application.Features.JobPosts.Queries.GetPagedJobs;
using Master.Application.Features.JobPosts.Queries.GetUserByJob;
using Master.Application.Models;
using Master.Features.JobPosts.Commands.DeleteJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.Api.Controllers;

/// <summary>
/// Controller responsible for managing job posts.
/// Provides endpoints for creating, updating, deleting, and retrieving job posts with filtering and pagination.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobPostController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobPostController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for CQRS pattern implementation.</param>
    public JobPostController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets the unique identifier of the currently authenticated user.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Retrieves a paged list of job posts based on the provided query parameters (Search, Sort, Pagination).
    /// </summary>
    /// <param name="query">The pagination and filter criteria.</param>
    /// <returns>A paged result containing job post data transfer objects.</returns>
    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaged([FromQuery] JobPostQuery query)
    {
        var result = await _mediator.Send(new GetPagedJobPostsQuery(query));
        return Ok(ApiResponse<PagedResult<JobPostResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Retrieves all available job posts without pagination.
    /// </summary>
    /// <returns>A collection of all job posts.</returns>
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetAllJobsQuery())));

    /// <summary>
    /// Retrieves the details of a specific job post by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <returns>The requested job post details.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new GetJobByIdQuery(id))));

    /// <summary>
    /// Gets all job posts created by the currently authenticated user, with an option to filter only active jobs.
    /// </summary>
    [HttpGet("myJobs")]
    public async Task<IActionResult> GetMyJobs()
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetJobsByUserIdQuery(UserId, true))));

    /// <summary>
    /// Get all job posts created by a specific user, with an option to filter only active jobs. This endpoint is accessible without authentication.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByUserId(Guid userId)
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetJobsByUserIdQuery(userId, false))));

    /// <summary>
    /// Gets the owner (customer) details of a specific job post by its unique identifier. This endpoint is accessible without authentication.
    /// </summary>
    [HttpGet("{id}/owner")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOwnerByJob(Guid id)
        => Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(await _mediator.Send(new GetUserByJobIdQuery(id))));

    /// <summary>
    /// Retrieves active job posts filtered by a specific required skill.
    /// </summary>
    /// <param name="skillId">The unique identifier of the required skill.</param>
    /// <returns>A collection of active job posts matching the skill.</returns>
    [HttpGet("bySkill/{skillId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySkill(Guid skillId)
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetActiveJobsBySkillQuery(skillId))));

    /// <summary>
    /// Creates a new job post for the currently authenticated user.
    /// </summary>
    /// <param name="request">The data required to create a job post.</param>
    /// <returns>The details of the newly created job post.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateJobPostDTO request)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new CreateJobCommand(UserId, request))));

    /// <summary>
    /// Updates an existing job post's details. Only the owner can perform this action.
    /// </summary>
    /// <param name="id">The unique identifier of the job post to update.</param>
    /// <param name="request">The updated job post information.</param>
    /// <returns>The updated job post details.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobPostDTO request)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new UpdateJobCommand(id, UserId, request))));

    /// <summary>
    /// Partially updates the status of a specific job post.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <param name="newStatus">The new status to be assigned (e.g., Active, Completed, Canceled).</param>
    /// <returns>A boolean value indicating whether the update was successful.</returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] JobPostStatus newStatus)
        => Ok(ApiResponse<bool>.SuccessResponse(await _mediator.Send(new ChangeJobStatusCommand(id, UserId, newStatus))));

    /// <summary>
    /// Deletes a specific job post. Only the owner of the job post is authorized to delete it.
    /// </summary>
    /// <param name="id">The unique identifier of the job post to delete.</param>
    /// <returns>A success message confirming deletion.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteJobCommand(id, UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Job post deleted successfully."));
    }

    /// <summary>
    /// Gets a list of all possible job statuses that can be assigned to a job post. This is useful for populating dropdowns or selection lists in the client application.
    /// </summary>
    /// <returns></returns>
    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
    => Ok(ApiResponse<List<JobStatusLookupDto>>.SuccessResponse(await _mediator.Send(new GetJobStatusLookupQuery())));
}