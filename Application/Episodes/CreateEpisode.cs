using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record CreateEpisodeCommand(
    Guid SeasonId,
    int EpisodeNumber,
    string Title,
    TimeSpan Duration) : IRequest<EpisodeResponse>;

public record EpisodeResponse(
    Guid Id,
    Guid SeasonId,
    int EpisodeNumber,
    string Title,
    string? Description,
    TimeSpan Duration,
    Uri VideoUrl,
    string? ThumbnailUrl,
    DateTime? ReleaseDateUtc,
    bool IsPublished);

public class CreateEpisodeCommandValidator : AbstractValidator<CreateEpisodeCommand>
{
    public CreateEpisodeCommandValidator()
    {
        RuleFor(x => x.SeasonId).NotEmpty().WithMessage("Season ID is required.");

        RuleFor(x => x.EpisodeNumber)
            .GreaterThan(0).WithMessage("Episode number must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Episode title cannot be empty.")
            .MaximumLength(500).WithMessage("Episode title must be at most 500 characters.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero.");
    }
}

public class CreateEpisodeHandler(
    ISeasonRepository seasonRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEpisodeCommand, EpisodeResponse>
{
    public async Task<EpisodeResponse> Handle(CreateEpisodeCommand request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.SeasonId, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.SeasonId));

        var episode = season.AddEpisode(request.EpisodeNumber, request.Title, request.Duration);

        await unitOfWork.SaveChangesAsync(ct);

        return new EpisodeResponse(
            episode.Id,
            episode.SeasonId,
            episode.EpisodeNumber,
            episode.Title,
            episode.Description,
            episode.Duration,
            episode.VideoUrl,
            episode.ThumbnailUrl,
            episode.ReleaseDateUtc,
            episode.IsPublished);
    }
}
