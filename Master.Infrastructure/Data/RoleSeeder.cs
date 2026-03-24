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

    private static readonly string[] FirstNames =
        {
            "Ali", "Murad", "Rashad", "Elvin", "Tural",
            "Kamran", "Nijat", "Samir", "Orkhan", "Farid"
        };

    private static readonly string[] LastNames =
        {
            "Aliyev", "Mammadov", "Huseynov", "Ismayilov", "Karimov",
            "Hasanov", "Guliyev", "Suleymanov", "Rahimov", "Abbasov"
        };
    private static readonly List<Skill> RealSkills = new()
        {
            new Skill { Name = "Plumbing", Description = "Fixing pipes, leaks, and water systems" },
            new Skill { Name = "Electrical Repair", Description = "Wiring, lighting, and electrical fixes" },
            new Skill { Name = "Carpentry", Description = "Woodwork, furniture, and repairs" },
            new Skill { Name = "Painting", Description = "Interior and exterior painting" },
            new Skill { Name = "HVAC Repair", Description = "Heating and cooling system repair" },
            new Skill { Name = "Cleaning Service", Description = "Home and office cleaning" },
            new Skill { Name = "Appliance Repair", Description = "Fixing washing machines, fridges, etc." },
            new Skill { Name = "Gardening", Description = "Garden maintenance and landscaping" },
            new Skill { Name = "Moving Service", Description = "House shifting and transport help" },
            new Skill { Name = "IT Support", Description = "Computer and network troubleshooting" }
        };

    private static readonly string[] JobTitles =
        {
            "Fix leaking kitchen sink",
            "Install new electrical sockets",
            "Paint 2-room apartment",
            "Repair washing machine",
            "Clean 3-bedroom house",
            "Assemble IKEA furniture",
            "Fix air conditioner",
            "Garden cleanup service",
            "Move furniture to new apartment",
            "Set up home WiFi network"
        };

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

        var adminEmail = "admin@Master.com";
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

        for (int i = 0; i < count; i++)
        {
            var firstName = FirstNames[rand.Next(FirstNames.Length)];
            var lastName = LastNames[rand.Next(LastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@mail.com";

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null) continue;

            var age = rand.Next(20, 50);

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Address = $"Baku, Azerbaijan",
                PhoneNumber = $"+99470{rand.Next(1000000, 9999999)}",
                EmailConfirmed = true,
                Experience = (short)rand.Next(1, 15),
                Age = (short)age,
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-age),
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
            context.Skills.AddRange(RealSkills);
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
            int skillCount = rand.Next(1, 4);
            var assignedSkills = skillsList.OrderBy(x => rand.Next()).Take(skillCount);

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

            for (int i = 0; i < jobCount; i++)
            {
                var randomSkill = skillsList[rand.Next(skillsList.Count)];
                var title = JobTitles[rand.Next(JobTitles.Length)];

                context.JobPosts.Add(new JobPost
                {
                    Title = title,
                    Description = $"Looking for professional to: {title.ToLower()}",
                    CustomerId = client.Id,
                    RequiredSkillId = randomSkill.Id,
                    JPStatus = JobPostStatus.Active,
                    CreatedDate = DateTime.UtcNow.AddDays(-rand.Next(0, 30)),
                    Budget = rand.Next(100, 3000)
                });
            }
        }

        await context.SaveChangesAsync();
    }
}