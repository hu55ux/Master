using Master.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Master.Domain.Models;

/// <summary>
/// Represents an application user in the system.
/// </summary>
/// <remarks>
/// This class extends <see cref="IdentityUser{TKey}"/> and includes additional
/// profile information such as personal details, experience, and relationships
/// with skills and job posts.
/// </remarks>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    /// <example>Smith</example>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's current GPS Latitude coordinate.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the user's current GPS Longitude coordinate.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the user's professional experience in years.
    /// </summary>
    /// <remarks>
    /// Represents the number of years the user has worked in their profession.
    /// </remarks>
    public short Experience { get; set; } = 0;

    /// <summary>
    /// Gets or sets the user's age.
    /// </summary>
    public short Age { get; set; }

    /// <summary>
    /// Gets or sets the user's date of birth.
    /// </summary>
    public DateTimeOffset DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the date when the user account was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the date when the user profile was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's profile picture URL hosted on Cloudinary CDN.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Average rating received by the master, calculated from all ratings given by customers.
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of ratings received by the master (Məsələn: 150).
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Gets or sets the current work availability status of the user (Available, Busy, Offline).
    /// </summary>
    public MasterStatus Status { get; set; } = MasterStatus.Available;

    /// <summary>
    /// Navigation property representing the skills owned by the user.
    /// </summary>
    /// <remarks>
    /// This is part of the many-to-many relationship between <see cref="AppUser"/> and <see cref="Skill"/>.
    /// </remarks>
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    /// <summary>
    /// Navigation property representing job posts created by the user.
    /// </summary>
    /// <remarks>
    /// These are the job requests posted by the user acting as a customer.
    /// </remarks>
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    /// <summary>
    /// User's received ratings as a master. This collection contains all the ratings given 
    /// by customers to this user in their role as a master.
    /// </summary>
    public virtual ICollection<MasterRating> ReceivedRatings { get; set; } = new List<MasterRating>();

    /// <summary>
    /// Given ratings by the user as a customer. This collection contains all the ratings that this user has given
    /// </summary>
    public virtual ICollection<MasterRating> GivenRatings { get; set; } = new List<MasterRating>();

    /// <summary>
    /// Chat rooms where this user acts as a customer.
    /// </summary>
    public virtual ICollection<ChatRoom> ChatRoomsAsCustomer { get; set; } = new List<ChatRoom>();

    /// <summary>
    /// Chat rooms where this user acts as a seller/master.
    /// </summary>
    public virtual ICollection<ChatRoom> ChatRoomsAsSeller { get; set; } = new List<ChatRoom>();

    /// <summary>
    /// Messages sent by this user across all chat rooms.
    /// </summary>
    public virtual ICollection<ChatMessage> SentChatMessages { get; set; } = new List<ChatMessage>();

    /// <summary>
    /// FCM Device token for sending mobile push notifications (iOS / Android / Web).
    /// </summary>
    public string? DeviceToken { get; set; }

    /// <summary>
    /// Mobile OS platform or device type (e.g. "android", "ios", "web").
    /// </summary>
    public string? DeviceType { get; set; }
}
