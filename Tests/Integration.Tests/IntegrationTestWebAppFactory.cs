using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Testcontainers.PostgreSql;

namespace Integration.Tests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("streaming_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    public async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Set<Domain.Associations.UserAnime>().RemoveRange(context.Set<Domain.Associations.UserAnime>());
        context.Set<Domain.Associations.AnimeGenre>().RemoveRange(context.Set<Domain.Associations.AnimeGenre>());
        context.Set<Domain.Associations.AnimeStudio>().RemoveRange(context.Set<Domain.Associations.AnimeStudio>());
        context.Set<Domain.Entities.Episode>().RemoveRange(context.Set<Domain.Entities.Episode>());
        context.Set<Domain.Entities.Season>().RemoveRange(context.Set<Domain.Entities.Season>());
        context.Set<Domain.Entities.Anime>().RemoveRange(context.Set<Domain.Entities.Anime>());
        context.Set<Domain.Entities.User>().RemoveRange(context.Set<Domain.Entities.User>());
        context.Set<Domain.Entities.Genre>().RemoveRange(context.Set<Domain.Entities.Genre>());
        context.Set<Domain.Entities.Studio>().RemoveRange(context.Set<Domain.Entities.Studio>());

        await context.SaveChangesAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            services.PostConfigure<CookieAuthenticationOptions>(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });
        });
    }
}
