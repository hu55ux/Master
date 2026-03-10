using Master.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Master.Data;

public class MasterDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();

            entity.Property(u => u.Address).HasMaxLength(250);

            entity.Property(u => u.Experience).HasDefaultValue(0);

            entity.Ignore(u => u.Age);

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });


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

        builder.Entity<RefreshToken>(
           refresh =>
           {
               refresh.HasKey(rt => rt.Id);
               refresh.HasIndex(rt => rt.JwtId).IsUnique();
               refresh.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
               refresh.Property(rt => rt.UserId).IsRequired().HasMaxLength(450);
           }
           );
    }
}
