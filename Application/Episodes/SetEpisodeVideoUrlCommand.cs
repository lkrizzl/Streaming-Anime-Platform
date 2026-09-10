using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record SetEpisodeVideoUrlCommand(Guid Id, Uri VideoUrl) : IRequest;

public class SetEpisodeVideoUrlCommandValidator : AbstractValidator<SetEpisodeVideoUrlCommand>
{
    public SetEpisodeVideoUrlCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.VideoUrl).NotNull();
    }
}

public class SetEpisodeVideoUrlHandler(
    IEpisodeRepository episodeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetEpisodeVideoUrlCommand>
{
    public async Task Handle(SetEpisodeVideoUrlCommand request, CancellationToken cancellationToken)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        episode.UpdateVideoUrl(request.VideoUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}