using Microsoft.EntityFrameworkCore;

namespace Master.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(connectionString));
        return services;
    }
}
