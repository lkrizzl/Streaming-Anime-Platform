using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApi.ServiceConfiguration;
public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Sreaming API",
                Version = "v1",
                Description = "API для платформи аніме стрімінгу"
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerMiddleware(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sreaming API v1");
            options.RoutePrefix = "swagger";
        });

        var baseUrl = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        app.Logger.LogInformation("🌐 Swagger UI: {Url}/swagger", baseUrl);

        return app;
    }
}