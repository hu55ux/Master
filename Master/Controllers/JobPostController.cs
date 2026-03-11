using Master.Common;
using Master.DTOs;
using Master.Models;
using Master.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// Controller responsible for managing Job Posts.
/// Provides endpoints for creating, updating, deleting and retrieving job posts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobPostController : ControllerBase
{
    private readonly IJobPostService _jobPostService;
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobPostController"/> class.
    /// </summary>
    /// <param name="jobPostService">Service responsible for job post operations.</param>
    /// <param name="authService">Authentication service used to access current user information.</param>
    public JobPostController(IJobPostService jobPostService, IAuthService authService)
    {
        _jobPostService = jobPostService;
        _authService = authService;
    }

    /// <summary>
    /// Retrieves all job posts.
    /// </summary>
    /// <returns>A list of all job posts.</returns>
    [HttpGet("All")]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetAll()
    {
        var jobs = await _jobPostService.GetAllJobsAsync();

        if (jobs is null || !jobs.Any())
        {
            return NotFound(
                ApiResponse<IEnumerable<JobPostResponseDTO>>
                .ErrorResponse("Jobs not found!")
            );
        }

        return Ok(
            ApiResponse<IEnumerable<JobPostResponseDTO>>
            .SuccessResponse(jobs)
        );
    }

    /// <summary>
    /// Retrieves a specific job post by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <returns>The requested job post.</returns>
    [HttpGet("get by:{id:guid}")]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> GetById(Guid id)
    {
        var job = await _jobPostService.GetJobByIdAsync(id);

        if (job is null)
            return NotFound(ApiResponse<object>.ErrorResponse($"Job with ID: {id} not found!"));

        return Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(job, "Job Post returned successfully!"));
    }

    /// <summary>
    /// Creates a new job post.
    /// </summary>
    /// <param name="request">Job post creation data.</param>
    /// <returns>The created job post.</returns>
    [HttpPost("Create")]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> Create([FromBody] CreateJobPostDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<Dictionary<string, string[]>>
                .ErrorResponse("Validation Error", ToValidationErrors(ModelState))
            );
        }

        var userId = _authService.UserId;

        var result = await _jobPostService.CreateJobAsync(userId, request);

        if (result == null)
        {
            return BadRequest(
                ApiResponse<JobPostResponseDTO>
                .ErrorResponse("Job could not be created")
            );
        }

        return Ok(
            ApiResponse<JobPostResponseDTO>
            .SuccessResponse(result, "Job created successfully")
        );
    }

    /// <summary>
    /// Updates an existing job post.
    /// </summary>
    /// <param name="jobId">The ID of the job post to update.</param>
    /// <param name="request">Updated job post data.</param>
    /// <returns>The updated job post.</returns>
    [HttpPut("update/{jobId:guid}")]
    public async Task<ActionResult<ApiResponse<JobPostResponseDTO>>> Update(
        Guid jobId,
        [FromBody] UpdateJobPostDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<Dictionary<string, string[]>>
                .ErrorResponse("Validation Error", ToValidationErrors(ModelState))
            );
        }

        var userId = _authService.UserId;

        var updatedJob = await _jobPostService.UpdateJobAsync(jobId, userId, request);

        if (updatedJob == null)
        {
            return NotFound(
                ApiResponse<object>
                .ErrorResponse($"Job with ID: {jobId} not found")
            );
        }

        return Ok(
            ApiResponse<JobPostResponseDTO>
            .SuccessResponse(updatedJob, "Job updated successfully")
        );
    }

    /// <summary>
    /// Deletes a job post.
    /// </summary>
    /// <param name="jobId">The ID of the job post to delete.</param>
    /// <returns>Success message.</returns>
    [HttpDelete("delete/{jobId:guid}")]
    public async Task<IActionResult> Delete(Guid jobId)
    {
        var userId = _authService.UserId;

        await _jobPostService.DeleteJobAsync(jobId, userId);

        return Ok(ApiResponse<object>.SuccessResponse("Job deleted successfully."));
    }

    /// <summary>
    /// Retrieves active job posts filtered by skill.
    /// </summary>
    /// <param name="skillId">Skill identifier used for filtering jobs.</param>
    /// <returns>A list of active jobs requiring the specified skill.</returns>
    [HttpGet("activeJP-by-skill{skillId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetActiveJobsBySkill(Guid skillId)
    {
        var result = await _jobPostService.GetActiveJobsBySkillAsync(skillId);

        if (result is null || !result.Any())
        {
            return NotFound(
                ApiResponse<IEnumerable<JobPostResponseDTO>>
                .ErrorResponse("Jobs not found!")
            );
        }

        return Ok(
            ApiResponse<IEnumerable<JobPostResponseDTO>>
            .SuccessResponse(result)
        );
    }

    /// <summary>
    /// Changes the status of a job post.
    /// </summary>
    /// <param name="id">The job post ID.</param>
    /// <param name="status">New job status (Active, Completed, Canceled, etc.).</param>
    /// <returns>Status update result.</returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] string status)
    {
        var userId = _authService.UserId;

        if (!TryParseJobStatus(status, out JobPostStatus parsedStatus))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid status value. Allowed: Active, Completed, Canceled, etc."));
        }

        var result = await _jobPostService.ChangeJobStatusAsync(id, userId, parsedStatus);

        return Ok(ApiResponse<object>.SuccessResponse("Status changed successfully"));
    }

    /// <summary>
    /// Retrieves job posts created by the currently authenticated user.
    /// </summary>
    /// <returns>A list of the user's job posts.</returns>
    [HttpGet("my-jobs")]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostResponseDTO>>>> GetMyJobs()
    {
        var userId = _authService.UserId;
        var result = await _jobPostService.GetMyJobsAsync(userId);

        if (result is null || !result.Any())
        {
            return NotFound(
                ApiResponse<IEnumerable<JobPostResponseDTO>>
                .ErrorResponse("Jobs not found!")
            );
        }

        return Ok(
            ApiResponse<IEnumerable<JobPostResponseDTO>>
            .SuccessResponse(result)
        );
    }

    /// <summary>
    /// Attempts to parse the provided string into a valid JobPostStatus enum value.
    /// </summary>
    private bool TryParseJobStatus(string status, out JobPostStatus result)
    {
        return Enum.TryParse<JobPostStatus>(status, true, out result) && Enum.IsDefined(typeof(JobPostStatus), result);
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