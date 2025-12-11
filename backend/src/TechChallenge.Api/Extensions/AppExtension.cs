using TechChallenge.Api.Middlewares;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Api.Extensions;

public static class AppExtension
{
    public static async Task<WebApplication> ConfigureAppAsync(this WebApplication app)
    {
        app.UseCors("AllowFrontend");

        await app.CreateAdminAsync();

        app.MapGet("/", () => "TechChallenge API is running");

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
        });

        app.AddMiddlewares();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
    
    public static async Task<WebApplication> CreateAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        
        await seeder.SeedAsync();

        return app;
    }

    public static void AddMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
