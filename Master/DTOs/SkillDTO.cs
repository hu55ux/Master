namespace Master.DTOs;

public class CreateSkillDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class UpdateSkillDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class SkillResponseDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
