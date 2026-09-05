using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Master.Application.Interfaces;
using Master.Application.Mapping;
using Master.Domain.Models;
using Master.Application.Validators;
using Master.Domain.Constants;
using Master.Infrastructure.BackgroundJobs;
using Master.Infrastructure.Config;
using Master.Infrastructure.Data;
using Master.Infrastructure.Repositories;
using Master.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace Master.API.Extensions;

/// <summary>
/// Extension methods for configuring services in the IServiceCollection.
/// Includes DbContext, Identity, JWT, Swagger, CORS, FluentValidation, AutoMapper, and application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the MasterDbContext to the service collection using the connection string from configuration.
    /// </summary>
    public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly("Master.Infrastructure")));

        return services;
    }

    /// <summary>
    /// Adds Swagger/OpenAPI support with JWT authentication and XML comments.
    /// </summary>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Master Service Finder API",
                Description = "An advanced API for a Service-Worker marketplace platform. " +
                              "This API facilitates user authentication, service provider (Master) skill management, " +
                              "and job posting management for customers. " +
                              "Built with ASP.NET Core and Identity Framework.",
                Contact = new OpenApiContact
                {
                    Name = "Master",
                    Email = "masterstepit@gmail.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT Licence",
                    Url = new Uri("https://opensource.org/license/mit")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = """
                    JWT Authorization header using the Bearer scheme. 
                    Example: Authorization: Bearer {token}
                    """,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.CustomSchemaIds(x => x.FullName);
        });

        return services;
    }

    /// <summary>
    /// Configures ASP.NET Identity with AppUser and IdentityRole.
    /// </summary>
    public static IServiceCollection AddIdentityAndDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<MasterDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<AwsConfig>(configuration.GetSection(AwsConfig.SectionName));

        return services;
    }

    /// <summary>
    /// Adds JWT-based authentication and authorization with policies.
    /// </summary>
    public static IServiceCollection AddJwtAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies["X-Access-Token"];
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        accessToken = context.Request.Query["access_token"];
                    }

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && 
                        (path.StartsWithSegments("/hubs") || path.StartsWithSegments("/api")))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.AdminOnly, policy => policy.RequireRole(UserRoles.Admin));
            options.AddPolicy(AuthPolicies.MasterOnly, policy => policy.RequireRole(UserRoles.Master));
            options.AddPolicy(AuthPolicies.ClientOnly, policy => policy.RequireRole(UserRoles.Client));
            options.AddPolicy(AuthPolicies.MasterOrAdmin, policy => policy.RequireRole(UserRoles.Master, UserRoles.Admin));
            options.AddPolicy(AuthPolicies.ClientOrAdmin, policy => policy.RequireRole(UserRoles.Client, UserRoles.Admin));
        });

        return services;
    }

    /// <summary>
    /// Adds CORS policy to allow requests from frontend applications.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevCors", policy =>
            {
                policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
    /// <summary>
    /// Adds FluentValidation support and registers validators from the assembly.
    /// </summary>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }

    /// <summary>
    /// Registers AutoMapper and application services (JobPostService, SkillService, AuthService, Chat, etc.).
    /// </summary>
    public static IServiceCollection AddAutoMapperAndOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();

        // Configure Redis Distributed Cache with fallback to Distributed Memory Cache if connection string is missing
        var redisConn = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConn))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "MasterChat_";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<TokenCleanupJob>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IJobPostRepository, JobPostRepository>();
        services.AddScoped<IMasterRatingRepository, MasterRatingRepository>();
        services.Configure<CloudinaryConfig>(configuration.GetSection(CloudinaryConfig.SectionName));
        services.AddScoped<IFileService, CloudinaryService>();

        // Chat & Push Notification Module Registrations
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IRedisChatService, RedisChatService>();
        services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();

        // Hybrid Location Module Registration
        services.AddHttpClient<ILocationService, LocationService>();

        return services;
    }

    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(connectionString));

        services.AddHangfireServer();

        return services;
    }
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            RecurringJob.AddOrUpdate<TokenCleanupJob>(
                "refresh-token-cleanup",
                job => job.DeleteRevokedTokens(),
                Cron.Weekly);
        }

        return app;
    }
}