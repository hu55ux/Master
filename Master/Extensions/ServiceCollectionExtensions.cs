using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Master.Config;
using Master.Data;
using Master.Mapping;
using Master.Models;
using Master.Services;
using Master.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Master.Extensions
{
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
                options.UseSqlServer(connectionString));
            return services;
        }

        /// <summary>
        /// Adds Swagger/OpenAPI support with JWT authentication and XML comments.
        /// </summary>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddControllers();

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
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MasterDbContext>()
            .AddDefaultTokenProviders();

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
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.AdminOnly, policy => policy.RequireRole(UserRoles.Admin));
                options.AddPolicy(AuthPolicies.MasterOnly, policy => policy.RequireRole(UserRoles.Master));
                options.AddPolicy(AuthPolicies.ClientOnly, policy => policy.RequireRole(UserRoles.Client));
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
                options.AddDefaultPolicy(policy => policy
                    .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000", "http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
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
        /// Registers AutoMapper and application services (JobPostService, SkillService, AuthService, etc.).
        /// </summary>
        public static IServiceCollection AddAutoMapperAndOtherServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IJobPostService, JobPostService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IAuthService, AuthService>();

            //services.AddScoped<IFileStorage, LocalDiskStorage>();
            //services.AddScoped<IAttachmentService, AttachmentService>();

            return services;
        }
    }
}