namespace Master.Models;

public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<UserSkill> UserSkills { get; set; }
    public ICollection<JobPost> JobPosts { get; set; }
}
