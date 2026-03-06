using Master.Models;
using Microsoft.AspNetCore.Identity;
namespace Master.Data;

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
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var roles = new[] { "Admin", "Master", "Client" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@master.com";
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
    }
}