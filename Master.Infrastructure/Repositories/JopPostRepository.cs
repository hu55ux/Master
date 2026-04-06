using Master.Application.Interfaces;
using Master.Domain.Models;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Repositories;

public class JobPostRepository : IJobPostRepository
{
    private readonly MasterDbContext _context;

    public JobPostRepository(MasterDbContext context) => _context = context;

    public async Task<JobPost?> GetByIdAndCustomerIdAsync(Guid jobId, Guid customerId, CancellationToken ct)
    {
        return await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == customerId, ct);
    }

    public void Update(JobPost job)
    {
        _context.JobPosts.Update(job);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task AddAsync(JobPost job, CancellationToken ct)
    {
        await _context.JobPosts.AddAsync(job, ct);
    }

    public async Task LoadReferencesAsync(JobPost job, CancellationToken ct)
    {
        await _context.Entry(job).Reference(j => j.Customer).LoadAsync(ct);

        if (job.RequiredSkillId != Guid.Empty && job.RequiredSkillId != null)
        {
            await _context.Entry(job).Reference(j => j.RequiredSkill).LoadAsync(ct);
        }
    }

    public void Remove(JobPost job)
    {
        _context.JobPosts.Remove(job);
    }

    public async Task<JobPost?> GetWithDetailsAsync(Guid jobId, Guid customerId, CancellationToken ct)
    {
        return await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CustomerId == customerId, ct);
    }

    public async Task<bool> SkillExistsAsync(Guid skillId, CancellationToken ct)
    {
        return await _context.Skills.AnyAsync(s => s.Id == skillId, ct);
    }

    public async Task<IEnumerable<JobPost>> GetActiveJobsBySkillAsync(Guid skillId, CancellationToken ct)
    {
        return await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .Where(j => j.JPStatus == JobPostStatus.Active && j.RequiredSkillId == skillId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JobPost>> GetAllWithDetailsAsync(CancellationToken ct)
    {
        return await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    public async Task<JobPost?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
    {
        return await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }
    public async Task<IEnumerable<JobPost>> GetJobsByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        return await _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .Where(j => j.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<(IEnumerable<JobPost> Items, int TotalCount)> GetPagedJobsAsync(JobPostQuery query, CancellationToken ct)
    {
        var jobQuery = _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim();
            jobQuery = jobQuery.Where(j =>
                j.Title.Contains(term) ||
                j.Description.Contains(term) ||
                j.Customer.UserName.Contains(term));
        }

        if (query.Status.HasValue)
        {
            jobQuery = jobQuery.Where(j => j.JPStatus == query.Status.Value);
        }

        jobQuery = ApplySorting(jobQuery, query.Sort, query.SortDirection);

        var totalCount = await jobQuery.CountAsync(ct);

        var skip = (query.Page - 1) * query.PageSize;
        var items = await jobQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    private IQueryable<JobPost> ApplySorting(IQueryable<JobPost> query, string sort, string direction)
    {
        var isDesc = direction.ToLower() == "desc";
        return sort?.ToLower() switch
        {
            "title" => isDesc ? query.OrderByDescending(j => j.Title) : query.OrderBy(j => j.Title),
            "status" => isDesc ? query.OrderByDescending(j => j.JPStatus) : query.OrderBy(j => j.JPStatus),
            "budget" => isDesc ? query.OrderByDescending(j => j.Budget) : query.OrderBy(j => j.Budget),
            "createddate" => isDesc ? query.OrderByDescending(j => j.CreatedDate) : query.OrderBy(j => j.CreatedDate),
            _ => query.OrderByDescending(j => j.Id)
        };
    }
    public async Task<IEnumerable<JobPost>> GetJobsByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct)
    {
        var query = _context.JobPosts
            .Include(j => j.Customer)
            .Include(j => j.RequiredSkill)
            .Where(j => j.CustomerId == userId);

        if (onlyActive)
        {
            query = query.Where(j => j.JPStatus == JobPostStatus.Active);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task<AppUser?> GetCustomerByJobIdAsync(Guid jobId, CancellationToken ct)
    {
        var job = await _context.JobPosts
            .Include(j => j.Customer)
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        return job?.Customer;
    }
}

