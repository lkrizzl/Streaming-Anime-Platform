using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AnimeRepository(AppDbContext dbContext) : IAnimeRepository
{
    private IQueryable<Anime> BaseQuery() => dbContext.Anime
        .Include(a => a.AnimeGenres)
            .ThenInclude(ag => ag.Genre)
        .Include(a => a.AnimeStudios)
            .ThenInclude(ast => ast.Studio);

    public async Task<Anime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await BaseQuery()
            .Include(a => a.Seasons.OrderBy(s => s.SeasonNumber.Value))
                .ThenInclude(s => s.Episodes.OrderBy(e => e.EpisodeNumber.Value))
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<PaginatedList<Anime>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = BaseQuery()
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedOnUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Anime>(items, page, pageSize, totalCount);
    }

    public async Task<PaginatedList<Anime>> GetAllAsync(int page, int pageSize, AnimeFilter filter, CancellationToken cancellationToken = default)
    {
        var query = BaseQuery()
            .AsNoTracking()
            .Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search}%";
            query = query.Where(a => EF.Functions.ILike(a.Title.Value, search)
                                  || EF.Functions.ILike(a.OriginalTitle.Value, search)
                                  || (a.EnglishTitle != null && EF.Functions.ILike(a.EnglishTitle.Value, search)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Genre))
        {
            var genre = filter.Genre;
            query = query.Where(a => a.AnimeGenres.Any(ag => EF.Functions.ILike(ag.Genre.Name.Value, genre)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        if (filter.ReleaseYear.HasValue)
        {
            query = query.Where(a => a.ReleaseYear.Value == filter.ReleaseYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Studio))
        {
            var studio = filter.Studio;
            query = query.Where(a => a.AnimeStudios.Any(ast => EF.Functions.ILike(ast.Studio.Name.Value, studio)));
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(a => a.CreatedOnUtc >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(a => a.CreatedOnUtc <= filter.ToDate.Value);
        }

        var sortBy = (filter.SortBy ?? "created").ToLowerInvariant();
        var sortOrder = (filter.SortOrder ?? "desc").ToLowerInvariant();
        var isDescending = sortOrder == "desc";

        query = (sortBy, isDescending) switch
        {
            ("title", false) => query.OrderBy(a => a.Title.Value),
            ("title", true) => query.OrderByDescending(a => a.Title.Value),
            ("rating", false) => query.OrderBy(a => a.AverageRating.Value),
            ("rating", true) => query.OrderByDescending(a => a.AverageRating.Value),
            ("year", false) => query.OrderBy(a => a.ReleaseYear.Value),
            ("year", true) => query.OrderByDescending(a => a.ReleaseYear.Value),
            _ when isDescending => query.OrderByDescending(a => a.CreatedOnUtc),
            _ => query.OrderBy(a => a.CreatedOnUtc),
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Anime>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        await dbContext.Anime.AddAsync(anime, cancellationToken);
    }

    public Task UpdateAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        dbContext.Anime.Update(anime);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        dbContext.Anime.Remove(anime);
        return Task.CompletedTask;
    }
}
