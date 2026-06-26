using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Animes;

public record DeleteAnimeCommand(Guid Id) : IRequest;

public class DeleteAnimeHandler(IAnimeRepository animeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAnimeCommand>
{
    public async Task Handle(DeleteAnimeCommand request, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.Id));

        await animeRepository.DeleteAsync(anime, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
