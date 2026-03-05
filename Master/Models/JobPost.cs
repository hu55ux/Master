namespace Master.Models;

public class JobPost
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal? Budget { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public Guid CustomerId { get; set; }
    public AppUser Customer { get; set; }
    public Guid RequiredSkillId { get; set; }
    public Skill RequiredSkill { get; set; }
}
