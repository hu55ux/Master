using Master.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwagger()
    .AddFluentValidation()
    .AddDataContext(builder.Configuration)
    .AddIdentityAndDb(builder.Configuration)
    .AddJwtAuthenticationAndAuthorization(builder.Configuration)
    .AddAutoMapperAndOtherServices();

var app = builder.Build();

app.UseMasterPiplene();

await app.EnsureSeededAsync();

app.Run();
