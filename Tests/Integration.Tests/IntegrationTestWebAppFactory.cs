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

    /// <summary>
    /// Apply pending migrations. Must be called after the container starts
    /// and before any test that needs the database.
    /// </summary>
    public async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Remove all data from all tables to give each test a clean slate.
    /// Called after every test via DisposeAsync in IntegrationTestBase.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Delete in dependency order to avoid FK violations
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
            // Remove the original DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Register DbContext with TestContainer connection string
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            // Configure cookie auth for testing (no HTTPS in tests)
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
