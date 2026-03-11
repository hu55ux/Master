namespace Master.Models;

public class JobPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public JobPostStatus JPStatus { get; set; } = JobPostStatus.Pending;
    public Guid CustomerId { get; set; }
    public AppUser? Customer { get; set; }
    public Guid RequiredSkillId { get; set; }
    public Skill? RequiredSkill { get; set; }
}

public enum JobPostStatus
{
    Pending,
    Active,
    InProgress,
    Completed,
    Canceled
}
