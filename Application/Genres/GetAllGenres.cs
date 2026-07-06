using Application.Abstractions;
using MediatR;

namespace Application.Genres;

// ====== Query ======

public record GetAllGenresQuery(int Page = 1, int PageSize = 50) : IRequest<PaginatedList<GenreResponse>>;

// ====== Handler ======

public class GetAllGenresHandler(IGenreRepository genreRepository)
    : IRequestHandler<GetAllGenresQuery, PaginatedList<GenreResponse>>
{
    public async Task<PaginatedList<GenreResponse>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var paginated = await genreRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

        var items = paginated.Items
            .Select(g => new GenreResponse(g.Id, g.Name, g.Description))
            .ToList();

        return new PaginatedList<GenreResponse>(items, paginated.Page, paginated.PageSize, paginated.TotalCount);
    }
}
