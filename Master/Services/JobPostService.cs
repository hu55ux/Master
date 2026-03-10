using AutoMapper;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Master.Services;

public class JobPostService : IJobPostService
{
    private readonly MasterDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public JobPostService(MasterDbContext context, UserManager<AppUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<IEnumerable<JobPostResponseDTO>> GetAllJobsAsync()
    {
        var jobs = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .ToListAsync();

        if (jobs is null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }

    public async Task<JobPostResponseDTO?> GetJobByIdAsync(Guid id)
    {
        var job = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            throw new KeyNotFoundException($"Job not found: {id}");

        return _mapper.Map<JobPostResponseDTO>(job);
    }

    public async Task<JobPost> GetJobEntityAsync(Guid id)
    {
        var job = await GetJobPostsWithIncludes()
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            throw new KeyNotFoundException($"Job not found: {id}");

        return job;
    }

    public async Task<JobPostResponseDTO> CreateJobAsync(Guid clientId, CreateJobPostDTO request)
    {
        var job = _mapper.Map<JobPost>(request);

        job.CustomerId = clientId;

        _context.JobPosts.Add(job);
        await _context.SaveChangesAsync();

        var createdJob = await GetJobPostsWithIncludes()
            .FirstOrDefaultAsync(j => j.Id == job.Id);

        return _mapper.Map<JobPostResponseDTO>(createdJob);
    }

    public async Task<bool> DeleteJobAsync(Guid jobId, Guid clientId)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job is null)
            throw new KeyNotFoundException($"Job posting not found or you do not have permission to delete: {jobId}");

        _context.JobPosts.Remove(job);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<JobPostResponseDTO> UpdateJobAsync(Guid jobId, Guid clientId, UpdateJobPostDTO request)
    {
        var job = await GetJobPostsWithIncludes()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job is null)
            throw new KeyNotFoundException($"Job not found: {jobId}");

        _mapper.Map(request, job);
        await _context.SaveChangesAsync();

        return _mapper.Map<JobPostResponseDTO>(job);
    }

    public async Task<IEnumerable<JobPostResponseDTO>> GetActiveJobsBySkillAsync(Guid skillId)
    {
        var jobs = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .Where(j => j.JPStatus == JobPostStatus.Active && j.RequiredSkillId == skillId)
            .ToListAsync();

        if (jobs is null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }

    public async Task<bool> ChangeJobStatusAsync(Guid jobId, Guid clientId, JobPostStatus newStatus)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == clientId);

        if (job is null)
            throw new KeyNotFoundException($"Job post not found or unauthorized: {jobId}");

        if (job.JPStatus == JobPostStatus.Completed || job.JPStatus == JobPostStatus.Canceled)
        {
            throw new InvalidOperationException("Cannot change status of a completed or canceled job.");
        }

        job.JPStatus = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> GetMyJobsAsync(Guid clientId)
    {
        var jobsForClient = await GetJobPostsWithIncludes()
            .AsNoTracking()
            .Where(j => j.CustomerId == clientId)
            .ToListAsync();

        if (jobsForClient == null || !jobsForClient.Any())
        {
            return Enumerable.Empty<JobPostResponseDTO>();
        }

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobsForClient);

    }

    private IQueryable<JobPost> GetJobPostsWithIncludes()
    {
        return _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill);
    }
}
