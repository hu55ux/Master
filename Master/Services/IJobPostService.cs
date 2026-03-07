using Master.DTOs;
using Master.Models;

namespace Master.Services;

public interface IJobPostService
{
    Task<IEnumerable<JobPostResponseDTO>> GetAllJobsAsync();
    Task<JobPostResponseDTO?> GetJobByIdAsync(Guid id);
    Task<IEnumerable<JobPostResponseDTO>> GetActiveJobsBySkillAsync(Guid skillId);
    Task<IEnumerable<JobPostResponseDTO>> GetMyJobsAsync(Guid clientId);
    Task<JobPostResponseDTO> CreateJobAsync(Guid clientId, CreateJobPostDTO request);
    Task<JobPostResponseDTO> UpdateJobAsync(Guid jobId, Guid clientId, UpdateJobPostDTO request);
    Task<bool> DeleteJobAsync(Guid jobId, Guid clientId);
    Task<bool> ChangeJobStatusAsync(Guid jobId, Guid clientId, JobPostStatus newStatus);
    Task<JobPost> GetJobEntityAsync(Guid id);
}