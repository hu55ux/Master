using Hangfire;
using Master.API.Middleware;
using Master.Infrastructure.Data;

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
            if (app.Environment.IsDevelopment())
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
                });
            }

            // Global exception handling middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Enable CORS policy
            app.UseCors();

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAdminAuthorizationFilter() },
                DashboardTitle = "Master API - Job Manager",
                AppPath = "/" // "Back to site" linki üçün
            });
            app.UseHangfireJobs();

            // Map controller endpoints
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

            // Seed default roles and users
            await RoleSeeder.SeedRolesAsync(serviceProvider);

            // Seed default skills and associated users
            await RoleSeeder.SeedSkillsAndUsersAsync(serviceProvider);
        }
    }
}