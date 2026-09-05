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
using Master.Domain.Models;
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
    /// <returns>A paged result containing job post DTOs.</returns>
    [HttpGet("paged")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<JobPostResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<JobPostResponseDTO>>>> GetPaged([FromQuery] JobPostQuery query)
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllJobsQuery());
        return Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Retrieves the details of a specific job post by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <returns>The requested job post details.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<JobPostResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(id));
        return Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets all job posts created by the currently authenticated user, with an option to filter only active jobs.
    /// </summary>
    [HttpGet("myJobs")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetMyJobs()
    {
        var result = await _mediator.Send(new GetJobsByUserIdQuery(UserId, true));
        return Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get all job posts created by a specific user, with an option to filter only active jobs.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetByUserId(Guid userId)
    {
        var result = await _mediator.Send(new GetJobsByUserIdQuery(userId, false));
        return Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets the owner (customer) details of a specific job post by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}/owner")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> GetOwnerByJob(Guid id)
    {
        var result = await _mediator.Send(new GetUserByJobIdQuery(id));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Retrieves active job posts filtered by a specific required skill.
    /// </summary>
    /// <param name="skillId">The unique identifier of the required skill.</param>
    /// <returns>A collection of active job posts matching the skill.</returns>
    [HttpGet("bySkill/{skillId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetBySkill(Guid skillId)
    {
        var result = await _mediator.Send(new GetActiveJobsBySkillQuery(skillId));
        return Ok(ApiResponse<IEnumerable<JobPostResponseDTO>>.SuccessResponse(result));
    }

    /// <summary>
    /// Creates a new job post for the currently authenticated user.
    /// </summary>
    /// <param name="request">The data required to create a job post.</param>
    /// <returns>The details of the newly created job post.</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<JobPostResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> Create([FromBody] CreateJobPostDTO request)
    {
        var result = await _mediator.Send(new CreateJobCommand(UserId, request));
        return Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(result, "Job post created successfully."));
    }

    /// <summary>
    /// Updates an existing job post's details. Only the owner can perform this action.
    /// </summary>
    /// <param name="id">The unique identifier of the job post to update.</param>
    /// <param name="request">The updated job post information.</param>
    /// <returns>The updated job post details.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<JobPostResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> Update(Guid id, [FromBody] UpdateJobPostDTO request)
    {
        var result = await _mediator.Send(new UpdateJobCommand(id, UserId, request));
        return Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(result, "Job post updated successfully."));
    }

    /// <summary>
    /// Partially updates the status of a specific job post.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <param name="newStatus">The new status to be assigned (e.g., Active, Completed, Canceled).</param>
    /// <returns>A boolean value indicating whether the update was successful.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(Guid id, [FromBody] JobPostStatus newStatus)
    {
        var result = await _mediator.Send(new ChangeJobStatusCommand(id, UserId, newStatus));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Job status changed successfully."));
    }

    /// <summary>
    /// Deletes a specific job post. Only the owner of the job post is authorized to delete it.
    /// </summary>
    /// <param name="id">The unique identifier of the job post to delete.</param>
    /// <returns>A success message confirming deletion.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
    {
        await _mediator.Send(new DeleteJobCommand(id, UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Job post deleted successfully."));
    }

    /// <summary>
    /// Gets a list of all possible job statuses that can be assigned to a job post (for dropdowns).
    /// </summary>
    /// <returns>Lookup options for job statuses.</returns>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(ApiResponse<List<JobStatusLookupDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<JobStatusLookupDto>>>> GetStatuses()
    {
        var result = await _mediator.Send(new GetJobStatusLookupQuery());
        return Ok(ApiResponse<List<JobStatusLookupDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Uploads one or multiple photo attachments to Cloudinary for a specific job post.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <param name="files">List of image files (jpg/png/webp) from form data.</param>
    /// <returns>List of uploaded Cloudinary CDN image URLs.</returns>
    [Authorize]
    [HttpPost("{id:guid}/images")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<string>>>> UploadJobPostImages(Guid id, [FromForm] List<IFormFile> files)
    {
        var result = await _mediator.Send(new Master.Application.Features.JobPosts.Commands.AddJobPostImages.AddJobPostImagesCommand(id, UserId, files));
        return Ok(ApiResponse<List<string>>.SuccessResponse(result, "Job post images uploaded to Cloudinary successfully."));
    }

    /// <summary>
    /// Deletes a specific photo attachment from a job post and removes it from Cloudinary.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <param name="imageId">The unique identifier of the image attachment to delete.</param>
    [Authorize]
    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteJobPostImage(Guid id, Guid imageId)
    {
        await _mediator.Send(new Master.Application.Features.JobPosts.Commands.DeleteJobPostImage.DeleteJobPostImageCommand(id, imageId, UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Job post image deleted successfully from Cloudinary."));
    }
}