using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record UpdateEpisodeCommand(
    Guid Id,
    string Title,
    TimeSpan Duration,
    string? Description,
    string? VideoUrl,
    string? ThumbnailUrl) : IRequest;

public class UpdateEpisodeCommandValidator : AbstractValidator<UpdateEpisodeCommand>
{
    public UpdateEpisodeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Episode ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Episode title cannot be empty.")
            .MaximumLength(500).WithMessage("Episode title must be at most 500 characters.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero.");

        RuleFor(x => x.VideoUrl)
            .MaximumLength(2048).WithMessage("Video URL must be at most 2048 characters.")
            .When(x => x.VideoUrl is not null);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2048).WithMessage("Thumbnail URL must be at most 2048 characters.")
            .When(x => x.ThumbnailUrl is not null);
    }
}

public class UpdateEpisodeHandler(
    IEpisodeRepository episodeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEpisodeCommand>
{
    public async Task Handle(UpdateEpisodeCommand request, CancellationToken ct)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        episode.UpdateTitle(request.Title);
        episode.UpdateDescription(request.Description);
        episode.UpdateDuration(request.Duration);

        if (request.VideoUrl is not null)
            episode.UpdateVideoUrl(request.VideoUrl);

        if (request.ThumbnailUrl is not null)
            episode.UpdateThumbnail(request.ThumbnailUrl);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
