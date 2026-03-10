using Master.Common;
using Master.DTOs;
using Master.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class JobPostController : ControllerBase
{
    private readonly IJobPostService _jobPostService;
    private readonly IAuthService _authService;

    public JobPostController(IJobPostService jobPostService, IAuthService authService)
    {
        _jobPostService = jobPostService;
        _authService = authService;
    }

    [HttpGet]
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

    [HttpGet("Get by: {Id}")]
    public async Task<ActionResult<JobPostResponseDTO>> GetById(Guid id)
    {
        var job = await _jobPostService.GetJobByIdAsync(id);

        if (job is null)
            return NotFound(ApiResponse<object>.ErrorResponse($"Job with ID: {id} not found!"));

        return Ok(ApiResponse<JobPostResponseDTO>.SuccessResponse(job, "Job Post returned successfully!"));
    }




    [HttpPost]
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


    [HttpPut("{jobId}")]
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

    [HttpDelete("{JobId}")]
    public async Task<IActionResult> Delete(Guid jobId)
    {
        var userId = _authService.UserId;

        await _jobPostService.DeleteJobAsync(jobId, userId);

        return Ok();
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