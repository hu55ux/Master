namespace Master.DTOs;

public class CreateJobPostDTO
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; } = 0!;
    public Guid RequiredSkillId { get; set; }
}

public class UpdateJobPostDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Budget { get; set; }
    public Guid? RequiredSkillId { get; set; }
}

public class JobPostResponseDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid RequiredSkillId { get; set; }
    public string RequiredSkillName { get; set; } = string.Empty;
}