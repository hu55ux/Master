using AutoMapper;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Microsoft.EntityFrameworkCore;

namespace Master.Services;

/// <summary>
/// Service responsible for managing job posts in the system.
/// </summary>
/// <remarks>
/// Handles business logic for creating, retrieving, updating,
/// deleting, and managing the status of job posts.
/// </remarks>
public class JobPostService : IJobPostService
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobPostService"/> class.
    /// </summary>
    /// <param name="context">Database context used for data access.</param>
    /// <param name="mapper">AutoMapper instance used for entity mapping.</param>
    public JobPostService(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    /// <summary>
    /// Gets all job posts in the system, including related customer and skill information.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<JobPostResponseDTO>> GetAllJobsAsync()
    {
        var jobs = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .ToListAsync();

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }

    /// <summary>
    /// Gets a specific job post by its unique identifier, including related customer and skill information.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<JobPostResponseDTO> GetJobByIdAsync(Guid id)
    {
        var job = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            throw new KeyNotFoundException($"Job with ID '{id}' was not found.");

        return _mapper.Map<JobPostResponseDTO>(job);
    }

    /// <summary>
    /// Gets a specific job post entity by its unique identifier, including related customer and skill information.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<JobPost> GetJobEntityAsync(Guid id)
    {
        var job = await GetJobPostsWithIncludes()
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            throw new KeyNotFoundException($"Job with ID '{id}' not found.");

        return job;
    }

    /// <summary>
    /// Creates a new job post for a specific client, setting the initial status to Active and associating it with the provided skill if specified.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<JobPostResponseDTO> CreateJobAsync(Guid clientId, CreateJobPostDTO request)
    {
        var job = _mapper.Map<JobPost>(request);

        job.CustomerId = clientId;
        job.JPStatus = JobPostStatus.Active;

        _context.JobPosts.Add(job);
        await _context.SaveChangesAsync();

        // Load related entities without a second query
        await _context.Entry(job).Reference(j => j.Customer).LoadAsync();
        if (job.RequiredSkillId != Guid.Empty)
        {
            await _context.Entry(job).Reference(j => j.RequiredSkill).LoadAsync();
        }

        return _mapper.Map<JobPostResponseDTO>(job);
    }

    /// <summary>
    /// Deletes a job post if it exists and belongs to the specified client. Throws an exception if the job is not found or if the client is unauthorized to delete it.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="clientId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<bool> DeleteJobAsync(Guid jobId, Guid clientId)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job == null)
            throw new KeyNotFoundException($"Job not found or unauthorized: {jobId}");

        _context.JobPosts.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates an existing job post if it exists and belongs to the specified client. Validates the existence of the specified skill if provided. Throws exceptions for not found, unauthorized access, or invalid skill references.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="clientId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<JobPostResponseDTO> UpdateJobAsync(Guid jobId, Guid clientId, UpdateJobPostDTO request)
    {
        var job = await GetJobPostsWithIncludes()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job == null)
            throw new KeyNotFoundException($"Job not found: {jobId}");

        if (request.RequiredSkillId.HasValue && request.RequiredSkillId != Guid.Empty)
        {
            var skillExists = await _context.Skills.AnyAsync(s => s.Id == request.RequiredSkillId.Value);
            if (!skillExists)
                throw new KeyNotFoundException("Specified Skill ID does not exist.");
        }

        _mapper.Map(request, job);
        await _context.SaveChangesAsync();

        if (job.RequiredSkillId != Guid.Empty)
            await _context.Entry(job).Reference(j => j.RequiredSkill).LoadAsync();

        return _mapper.Map<JobPostResponseDTO>(job);
    }

    /// <summary>
    /// Gets all active job posts that require a specific skill, including related customer and skill information. Returns an empty collection if no matching jobs are found.
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<JobPostResponseDTO>> GetActiveJobsBySkillAsync(Guid skillId)
    {
        var jobs = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .Where(j => j.JPStatus == JobPostStatus.Active && j.RequiredSkillId == skillId)
            .ToListAsync();

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }

    /// <summary>
    /// Changes the status of a job post if it exists, belongs to the specified client, and is not already completed or canceled. Throws exceptions for not found, unauthorized access, or invalid status transitions.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="clientId"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> ChangeJobStatusAsync(Guid jobId, Guid clientId, JobPostStatus newStatus)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job == null)
            throw new KeyNotFoundException($"Job not found or unauthorized: {jobId}");

        if (job.JPStatus == JobPostStatus.Completed || job.JPStatus == JobPostStatus.Canceled)
            throw new InvalidOperationException("Cannot change status of a completed or canceled job.");

        if (job.JPStatus == newStatus)
            return true;

        job.JPStatus = newStatus;
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// Gets all job posts created by a specific client, including related customer and skill information. Returns an empty collection if the client has not created any jobs.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<JobPostResponseDTO>> GetMyJobsAsync(Guid clientId)
    {
        var jobsForClient = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .Where(j => j.CustomerId == clientId)
            .ToListAsync();

        if (jobsForClient == null || !jobsForClient.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobsForClient);
    }

    /// <summary>
    /// Returns a queryable collection of job posts including related entities.
    /// </summary>
    /// <remarks>
    /// Ensures related data such as the customer and required skill are included in the query.
    /// </remarks>
    /// <returns>A queryable job post collection.</returns>
    private IQueryable<JobPost> GetJobPostsWithIncludes()
    {
        return _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill);
    }
}