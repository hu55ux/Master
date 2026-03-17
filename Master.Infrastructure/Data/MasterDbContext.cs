using Master.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Data;

/// <summary>
/// Represents the Entity Framework database context for the Master application.
/// Inherits from IdentityDbContext to include user authentication and authorization features.
/// </summary>
public class MasterDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MasterDbContext"/> with specified options.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    { }

    /// <summary>
    /// Gets or sets the skills in the system.
    /// </summary>
    public DbSet<Skill> Skills => Set<Skill>();

    /// <summary>
    /// Gets or sets the job posts in the system.
    /// </summary>
    public DbSet<JobPost> JobPosts => Set<JobPost>();

    /// <summary>
    /// Gets or sets the user-skill relationships.
    /// </summary>
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();

    /// <summary>
    /// Gets or sets the refresh tokens for authentication.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Configures the schema needed for the Master database context.
    /// </summary>
    /// <param name="builder">The model builder to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AppUser configuration
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Address).HasMaxLength(250);
            entity.Property(u => u.Experience).HasDefaultValue(0);
            entity.Ignore(u => u.Age);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // UserSkill configuration (many-to-many between User and Skill)
        builder.Entity<UserSkill>()
            .HasKey(us => new { us.UserId, us.SkillId });

        builder.Entity<UserSkill>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSkills)
            .HasForeignKey(us => us.UserId);

        builder.Entity<UserSkill>()
            .HasOne(us => us.Skill)
            .WithMany(s => s.UserSkills)
            .HasForeignKey(us => us.SkillId);

        // JobPost configuration
        builder.Entity<JobPost>()
            .HasOne(jp => jp.Customer)
            .WithMany(u => u.JobPosts)
            .HasForeignKey(jp => jp.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobPost>()
            .Property(j => j.Budget)
            .HasColumnType("decimal(18,2)");

        builder.Entity<JobPost>()
            .HasOne(jp => jp.RequiredSkill)
            .WithMany(s => s.JobPosts)
            .HasForeignKey(jp => jp.RequiredSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobPost>()
            .Property(j => j.JPStatus)
            .HasConversion<string>();

        // RefreshToken configuration
        builder.Entity<RefreshToken>(refresh =>
        {
            refresh.HasKey(rt => rt.Id);
            refresh.HasIndex(rt => rt.JwtId).IsUnique();
            refresh.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
            refresh.Property(rt => rt.UserId).IsRequired().HasMaxLength(450);
        });
    }
}