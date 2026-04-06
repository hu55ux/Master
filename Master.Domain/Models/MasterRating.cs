namespace Master.Domain.Models;

public class MasterRating
{
    public Guid MasterId { get; set; }
    public virtual AppUser? Master { get; set; }
    public Guid CustomerId { get; set; }
    public virtual AppUser? Customer { get; set; }
    public decimal Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}