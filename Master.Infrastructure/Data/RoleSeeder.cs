using Master.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Master.Infrastructure.Data;

/// <summary>
/// Seeds initial roles and a default admin user into the database for the Invoice Management application.
/// </summary>
public static class RoleSeeder
{
    /// <summary>
    /// Constructs the initial roles ("Admin", "User") and a default admin user with the email "
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var roles = new[] { "Admin", "Master", "Client" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminEmail = "admin@Master.Infrastructurecom";
        var adminPassword = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "System",
                Address = "Memmed E. st,Sumqayit",
                PhoneNumber = "+994775647477",
                EmailConfirmed = true,
                DateOfBirth = new DateTimeOffset(new DateTime(2004, 01, 21)),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = null
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
        await CreateRandomUsers(userManager, "Master", 20);
        await CreateRandomUsers(userManager, "Client", 20);

    }
    private static async Task CreateRandomUsers(UserManager<AppUser> userManager, string role, int count)
    {
        var rand = new Random();

        for (int i = 1; i <= count; i++)
        {
            var email = $"{role.ToLower()}{i}@mail.com";
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null) continue;

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = $"{role}First{i}",
                LastName = $"{role}Last{i}",
                Address = $"Street {i}",
                PhoneNumber = $"+99477{rand.Next(1000000, 9999999)}",
                EmailConfirmed = true,
                Experience = (short)rand.Next(0, 10),
                Age = (short)rand.Next(20, 50),
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-(rand.Next(20, 50))),
                CreatedAt = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
    public static async Task SeedSkillsAndUsersAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<MasterDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var rand = new Random();

        if (!context.Skills.Any())
        {
            var skills = new List<Skill>();
            for (int i = 1; i <= 50; i++)
            {
                skills.Add(new Skill
                {
                    Name = $"Skill {i}",
                    Description = $"Description for Skill {i}"
                });
            }
            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();
        }

        var skillsList = await context.Skills.ToListAsync();

        var allUsers = userManager.Users.ToList();
        var masters = new List<AppUser>();
        var clients = new List<AppUser>();

        foreach (var user in allUsers)
        {
            if (await userManager.IsInRoleAsync(user, "Master"))
                masters.Add(user);
            else if (await userManager.IsInRoleAsync(user, "Client"))
                clients.Add(user);
        }

        if (!masters.Any()) throw new Exception("No Master users found.");
        if (!clients.Any()) throw new Exception("No Client users found.");

        foreach (var master in masters)
        {
            int skillCount = rand.Next(1, 6);
            var assignedSkills = skillsList.OrderBy(s => rand.Next()).Take(skillCount).ToList();

            foreach (var skill in assignedSkills)
            {
                if (!context.UserSkills.Any(us => us.UserId == master.Id && us.SkillId == skill.Id))
                {
                    context.UserSkills.Add(new UserSkill
                    {
                        UserId = master.Id,
                        SkillId = skill.Id
                    });
                }
            }
        }

        foreach (var client in clients)
        {
            int jobCount = rand.Next(1, 4);
            for (int i = 1; i <= jobCount; i++)
            {
                var randomSkill = skillsList[rand.Next(skillsList.Count)];
                context.JobPosts.Add(new JobPost
                {
                    Title = $"Job {i} for {client.FirstName}",
                    Description = $"Description for Job {i}",
                    CustomerId = client.Id,
                    RequiredSkillId = randomSkill.Id,
                    JPStatus = (JobPostStatus)rand.Next(Enum.GetValues(typeof(JobPostStatus)).Length),
                    CreatedDate = DateTime.UtcNow.AddDays(-rand.Next(0, 30)),
                    Budget = rand.Next(100, 5000)
                });
            }
        }

        await context.SaveChangesAsync();
    }
}