using Hangfire;
using Master.API.Middleware;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring the application's HTTP request pipeline.
    /// Includes Swagger, global exception middleware, CORS, authentication, authorization, and seeding.
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// Configures the middleware pipeline for the Master application.
        /// Enables Swagger in development, global exception handling, CORS, authentication, authorization, and maps controllers.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        /// <returns>The configured WebApplication instance.</returns>
        public static WebApplication UseMasterPipeline(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Master API v1");

                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableDeepLinking();
                options.EnableTryItOutByDefault();
                options.EnablePersistAuthorization();

                //options.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseRouting();

            app.UseCors("DevCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAdminAuthorizationFilter() },
                DashboardTitle = "Master API - Job Manager",
                AppPath = "/"
            });
            app.UseHangfireJobs();

            app.MapControllers();

            return app;
        }

        /// <summary>
        /// Seeds default roles, skills, and users into the database if they do not already exist.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        public static async Task EnsureSeededAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var context = serviceProvider.GetRequiredService<MasterDbContext>();

                await context.Database.MigrateAsync();

                await RoleSeeder.SeedRolesAsync(serviceProvider);

                await RoleSeeder.SeedSkillsAndUsersAsync(serviceProvider);

                await RoleSeeder.SeedRatingsAsync(serviceProvider);
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}