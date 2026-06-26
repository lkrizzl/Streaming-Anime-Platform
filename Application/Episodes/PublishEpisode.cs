using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record PublishEpisodeCommand(Guid Id, DateTime? ReleaseDate = null) : IRequest;

public class PublishEpisodeCommandValidator : AbstractValidator<PublishEpisodeCommand>
{
    public PublishEpisodeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Episode ID is required.");
    }
}

public class PublishEpisodeHandler(
    IEpisodeRepository episodeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PublishEpisodeCommand>
{
    public async Task Handle(PublishEpisodeCommand request, CancellationToken ct)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        episode.Publish(request.ReleaseDate);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
