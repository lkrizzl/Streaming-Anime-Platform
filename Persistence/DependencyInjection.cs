using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Services;

namespace Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnimeRepository, AnimeRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<IUserAnimeRepository, UserAnimeRepository>();
        services.AddScoped<IUserIdentityService, Services.UserIdentityService>();

        return services;
    }
}
