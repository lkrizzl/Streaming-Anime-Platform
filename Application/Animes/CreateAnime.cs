using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Animes;

public record CreateAnimeCommand(
    string Title,
    string OriginalTitle,
    string? EnglishTitle,
    string Description,
    int ReleaseYear,
    AnimeStatus Status,
    string? CoverImageUrl,
    string? BannerImageUrl,
    string? TrailerUrl,
    string? AgeRating,
    List<string> Genres,
    List<string> Studios
) : IRequest<AnimeResponse>;

public record AnimeResponse(
    Guid Id,
    string Title,
    string OriginalTitle,
    string? EnglishTitle,
    string Description,
    int ReleaseYear,
    AnimeStatus Status,
    string? CoverImageUrl,
    string? BannerImageUrl,
    string? TrailerUrl,
    string? AgeRating,
    double AverageRating,
    int RatingCount,
    int EpisodesCount,
    bool IsActive,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc,
    List<string> Genres,
    List<string> Studios
);

public class CreateAnimeCommandValidator : AbstractValidator<CreateAnimeCommand>
{
    public CreateAnimeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(500).WithMessage("Title must be at most 500 characters.");

        RuleFor(x => x.OriginalTitle)
            .NotEmpty().WithMessage("Original title cannot be empty.")
            .MaximumLength(500).WithMessage("Original title must be at most 500 characters.");

        RuleFor(x => x.EnglishTitle)
            .MaximumLength(500).WithMessage("English title must be at most 500 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description cannot be empty.")
            .MaximumLength(5000).WithMessage("Description must be at most 5000 characters.");

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 5)
            .WithMessage($"Release year must be between 1900 and {DateTime.UtcNow.Year + 5}.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid anime status.");

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(2048).WithMessage("Cover image URL must be at most 2048 characters.")
            .When(x => x.CoverImageUrl is not null);

        RuleFor(x => x.BannerImageUrl)
            .MaximumLength(2048).WithMessage("Banner image URL must be at most 2048 characters.")
            .When(x => x.BannerImageUrl is not null);

        RuleFor(x => x.TrailerUrl)
            .MaximumLength(2048).WithMessage("Trailer URL must be at most 2048 characters.")
            .When(x => x.TrailerUrl is not null);

        RuleFor(x => x.AgeRating)
            .MaximumLength(20).WithMessage("Age rating must be at most 20 characters.")
            .When(x => x.AgeRating is not null);

        RuleFor(x => x.Genres)
            .NotNull().WithMessage("Genres list cannot be null.")
            .Must(g => g.Count > 0).WithMessage("At least one genre is required.");

        RuleFor(x => x.Studios)
            .NotNull().WithMessage("Studios list cannot be null.")
            .Must(s => s.Count > 0).WithMessage("At least one studio is required.");
    }
}

public class CreateAnimeHandler(
    IAnimeRepository animeRepository,
    IGenreRepository genreRepository,
    IStudioRepository studioRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAnimeCommand, AnimeResponse>
{
    public async Task<AnimeResponse> Handle(CreateAnimeCommand request, CancellationToken ct)
    {
        var anime = new Domain.Entities.Anime(
            request.Title,
            request.OriginalTitle,
            request.Description,
            request.ReleaseYear,
            request.Status);

        if (request.EnglishTitle is not null)
            anime.UpdateEnglishTitle(request.EnglishTitle);

        if (request.CoverImageUrl is not null)
            anime.SetCoverImage(request.CoverImageUrl);

        if (request.BannerImageUrl is not null)
            anime.SetBannerImage(request.BannerImageUrl);

        if (request.TrailerUrl is not null)
            anime.SetTrailerUrl(request.TrailerUrl);

        if (request.AgeRating is not null)
            anime.SetAgeRating(request.AgeRating);

        // Resolve and attach genres
        foreach (var genreName in request.Genres)
        {
            var genre = await genreRepository.GetByNameAsync(genreName, ct)
                ?? throw new NotFoundException(AnimeErrors.GenreNotFoundInSystem(genreName));

            anime.AddGenre(genre);
        }

        // Resolve and attach studios
        foreach (var studioName in request.Studios)
        {
            var studio = await studioRepository.GetByNameAsync(studioName, ct)
                ?? throw new NotFoundException(AnimeErrors.StudioNotFoundInSystem(studioName));

            anime.AddStudio(studio);
        }

        await animeRepository.AddAsync(anime, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapResponse(anime);
    }

    private static AnimeResponse MapResponse(Domain.Entities.Anime anime) => new(
        anime.Id,
        anime.Title,
        anime.OriginalTitle,
        anime.EnglishTitle,
        anime.Description,
        anime.ReleaseYear,
        anime.Status,
        anime.CoverImageUrl,
        anime.BannerImageUrl,
        anime.TrailerUrl,
        anime.AgeRating,
        anime.AverageRating,
        anime.RatingCount,
        anime.EpisodesCount,
        anime.IsActive,
        anime.CreatedOnUtc,
        anime.UpdatedOnUtc,
        anime.Genres.Select(g => g.Name).ToList(),
        anime.Studios.Select(s => s.Name).ToList()
    );
}
