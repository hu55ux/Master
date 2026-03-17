using Hangfire.Dashboard;

namespace Master.Middleware
{
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;
            var isAdmin = httpContext.User.IsInRole("Admin");

            return isAuthenticated && isAdmin;
        }
    }
}