namespace Master.Models;

public class UserSkill
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; }
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; }
}
