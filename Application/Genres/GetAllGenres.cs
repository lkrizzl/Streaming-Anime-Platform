using Application.Abstractions;
using MediatR;

namespace Application.Genres;

// ====== Query ======

public record GetAllGenresQuery : IRequest<IReadOnlyList<GenreResponse>>;

// ====== Handler ======

public class GetAllGenresHandler(IGenreRepository genreRepository)
    : IRequestHandler<GetAllGenresQuery, IReadOnlyList<GenreResponse>>
{
    public async Task<IReadOnlyList<GenreResponse>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await genreRepository.GetAllAsync(cancellationToken);

        return genres
            .Select(g => new GenreResponse(g.Id, g.Name, g.Description))
            .ToList();
    }
}
