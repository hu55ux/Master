using Master.DTOs;
using Master.Models;

namespace Master.Services;

/// <summary>
/// Defines operations related to job post management.
/// </summary>
/// <remarks>
/// This service handles business logic for creating, updating, retrieving,
/// and deleting job posts in the system.
/// </remarks>
public interface IJobPostService
{
    /// <summary>
    /// Retrieves all job posts in the system.
    /// </summary>
    /// <returns>A collection of job post response DTOs.</returns>
    Task<IEnumerable<JobPostResponseDTO>> GetAllJobsAsync();

    /// <summary>
    /// Retrieves a specific job post by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the job post.</param>
    /// <returns>The job post if found; otherwise null.</returns>
    Task<JobPostResponseDTO?> GetJobByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all active job posts that require a specific skill.
    /// </summary>
    /// <param name="skillId">The identifier of the required skill.</param>
    /// <returns>A collection of active job posts.</returns>
    Task<IEnumerable<JobPostResponseDTO>> GetActiveJobsBySkillAsync(Guid skillId);

    /// <summary>
    /// Retrieves all job posts created by a specific client.
    /// </summary>
    /// <param name="clientId">The identifier of the client.</param>
    /// <returns>A collection of the client's job posts.</returns>
    Task<IEnumerable<JobPostResponseDTO>> GetMyJobsAsync(Guid clientId);

    /// <summary>
    /// Creates a new job post.
    /// </summary>
    /// <param name="clientId">The identifier of the client creating the job.</param>
    /// <param name="request">The job post creation data.</param>
    /// <returns>The created job post.</returns>
    Task<JobPostResponseDTO> CreateJobAsync(Guid clientId, CreateJobPostDTO request);

    /// <summary>
    /// Updates an existing job post.
    /// </summary>
    /// <param name="jobId">The identifier of the job post to update.</param>
    /// <param name="clientId">The identifier of the client attempting the update.</param>
    /// <param name="request">The updated job post data.</param>
    /// <returns>The updated job post.</returns>
    Task<JobPostResponseDTO> UpdateJobAsync(Guid jobId, Guid clientId, UpdateJobPostDTO request);

    /// <summary>
    /// Deletes a job post created by a client.
    /// </summary>
    /// <param name="jobId">The identifier of the job post.</param>
    /// <param name="clientId">The identifier of the client requesting the deletion.</param>
    /// <returns>True if the job was successfully deleted; otherwise false.</returns>
    Task<bool> DeleteJobAsync(Guid jobId, Guid clientId);

    /// <summary>
    /// Changes the status of a job post.
    /// </summary>
    /// <param name="jobId">The identifier of the job post.</param>
    /// <param name="clientId">The identifier of the client requesting the status change.</param>
    /// <param name="newStatus">The new status to assign to the job post.</param>
    /// <returns>True if the status change was successful; otherwise false.</returns>
    Task<bool> ChangeJobStatusAsync(Guid jobId, Guid clientId, JobPostStatus newStatus);

    /// <summary>
    /// Retrieves the job post entity directly from the database.
    /// </summary>
    /// <param name="id">The identifier of the job post.</param>
    /// <returns>The job post entity.</returns>
    /// <remarks>
    /// This method returns the entity itself instead of a DTO and is typically
    /// used internally for business logic operations.
    /// </remarks>
    Task<JobPost> GetJobEntityAsync(Guid id);
}