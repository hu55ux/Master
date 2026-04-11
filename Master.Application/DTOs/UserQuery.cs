namespace Master.Application.DTOs;

public class UserQuery
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
