using System.Security.Claims;
using Master.Common;
using Master.DTOs;
using Master.Features.JobPosts.Commands.ChangejobStatus;
using Master.Features.JobPosts.Commands.CreateJob;
using Master.Features.JobPosts.Commands.DeleteJob;
using Master.Features.JobPosts.Commands.UpdateJob;
using Master.Features.JobPosts.Queries.GetActiveJobs;
using Master.Features.JobPosts.Queries.GetAllJobs;
using Master.Features.JobPosts.Queries.GetJobById;
using Master.Features.JobPosts.Queries.GetMyJobs;
using Master.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller responsible for managing Job Posts.
/// Provides endpoints for creating, updating, deleting and retrieving job posts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class JobPostController : ControllerBase
{
    /// <summary>
    /// Mediator instance for handling commands and queries related to job posts.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor for JobPostController, initializes the mediator instance.
    /// </summary>
    /// <param name="mediator"></param>
    public JobPostController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// UserId property retrieves the current user's ID from the claims. This is used for operations that require user context, such as creating or updating job posts.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Gets all job posts. This endpoint is accessible to anonymous users, allowing anyone to view available job posts without authentication.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetAllJobsQuery())));

    /// <summary>
    /// Gets a specific job post by its ID. This endpoint is also accessible to anonymous users, allowing anyone to view the details of a specific job post without authentication.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new GetJobByIdQuery(id))));

    /// <summary>
    /// MyJobs endpoint retrieves all job posts created by the currently authenticated user. This allows users to manage their own job posts, view their status,
    /// and make updates as needed. The endpoint requires authentication to ensure that users can only access their own job posts.
    /// </summary>
    /// <returns></returns>
    [HttpGet("myJobs")]
    public async Task<IActionResult> GetMyJobs()
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetMyJobsQuery(UserId))));

    /// <summary>
    /// Gets active job posts that require a specific skill. This endpoint allows users to find job posts that match their skills and interests. 
    /// It is accessible to anonymous users, enabling anyone to search for jobs based on required skills without authentication.
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    [HttpGet("bySkill/{skillId}")]
    public async Task<IActionResult> GetBySkill(Guid skillId)
        => Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(await _mediator.Send(new GetActiveJobsBySkillQuery(skillId))));

    /// <summary>
    /// Creates a new job post. This endpoint requires authentication, as only authenticated users can create job posts. The request body should contain the necessary details for the job post, 
    /// such as title, description, budget, and required skills. Upon successful creation, the endpoint returns the details of the newly created job post.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateJobPostDTO request)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new CreateJobCommand(UserId, request))));

    /// <summary>
    /// Updates an existing job post. This endpoint requires authentication, as only the owner of the job 
    /// post can update it. The request body should contain the updated details for the job post,
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobPostDTO request)
        => Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(await _mediator.Send(new UpdateJobCommand(id, UserId, request))));

    /// <summary>
    /// Changes the status of a job post. This endpoint allows the owner of the job post to update its status (e.g., from Active to InProgress or Completed).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] JobPostStatus newStatus)
        => Ok(ApiResponse<bool>.SuccessResponse(await _mediator.Send(new ChangeJobStatusCommand(id, UserId, newStatus))));

    /// <summary>
    /// Deletes a job post. This endpoint requires authentication, as only the owner of the job post can delete it. 
    /// Upon successful deletion, the endpoint returns a success message confirming that the job post has been deleted.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteJobCommand(id, UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Job post deleted successfully."));
    }
}