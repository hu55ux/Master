using Master.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwagger()
    .AddFluentValidation()
    .AddDataContext(builder.Configuration)
    .AddIdentityAndDb(builder.Configuration)
    .AddJwtAuthenticationAndAuthorization(builder.Configuration)
    .AddAutoMapperAndOtherServices(builder.Configuration)
    .AddHangfireServices(builder.Configuration);

var app = builder.Build();

app.UseMasterPipeline();

//  await app.EnsureSeededAsync();

app.Run();
