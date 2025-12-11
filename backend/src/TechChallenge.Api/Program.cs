using TechChallenge.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApiAndSwagger()
    .AddDomainServices()
    .AddDataAccess(builder.Configuration)
    .AddMediatorAndPipelines()
    .AddJwtAuth(builder.Configuration)
    .AddCorsFrontend();

var app = builder.Build();

await app.ConfigureAppAsync();

app.Run();