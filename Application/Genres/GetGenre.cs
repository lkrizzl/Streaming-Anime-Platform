using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Genres;
public record GetGenreQuery(Guid Id) : IRequest<GenreResponse>;
public class GetGenreHandler(IGenreRepository genreRepository)
    : IRequestHandler<GetGenreQuery, GenreResponse>
{
    public async Task<GenreResponse> Handle(GetGenreQuery request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(GenreErrors.GenreNotFound(request.Id));

        return new GenreResponse(genre.Id, genre.Name, genre.Description);
    }
}
