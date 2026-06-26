using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Animes;

public record UpdateAnimeCommand(
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
    List<string> Genres,
    List<string> Studios
) : IRequest;

public class UpdateAnimeCommandValidator : AbstractValidator<UpdateAnimeCommand>
{
    public UpdateAnimeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Anime ID is required.");

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

public class UpdateAnimeHandler(
    IAnimeRepository animeRepository,
    IGenreRepository genreRepository,
    IStudioRepository studioRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAnimeCommand>
{
    public async Task Handle(UpdateAnimeCommand request, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.Id));

        anime.UpdateTitle(request.Title);
        anime.UpdateOriginalTitle(request.OriginalTitle);
        anime.UpdateEnglishTitle(request.EnglishTitle);
        anime.UpdateDescription(request.Description);

        anime.SetCoverImage(request.CoverImageUrl);
        anime.SetBannerImage(request.BannerImageUrl);
        anime.SetTrailerUrl(request.TrailerUrl);
        anime.SetAgeRating(request.AgeRating);

        // Sync genres
        var currentGenreIds = anime.AnimeGenres.Select(ag => ag.GenreId).ToHashSet();

        var newGenres = new List<Domain.Entities.Genre>();
        foreach (var genreName in request.Genres)
        {
            var genre = await genreRepository.GetByNameAsync(genreName, ct)
                ?? throw new NotFoundException(AnimeErrors.GenreNotFoundInSystem(genreName));
            newGenres.Add(genre);
        }

        var newGenreIds = newGenres.Select(g => g.Id).ToHashSet();

        // Remove genres not in new list
        foreach (var ag in anime.AnimeGenres.Where(ag => !newGenreIds.Contains(ag.GenreId)).ToList())
            anime.RemoveGenre(ag.GenreId);

        // Add genres not in current list
        foreach (var genre in newGenres.Where(g => !currentGenreIds.Contains(g.Id)))
            anime.AddGenre(genre);

        // Sync studios
        var currentStudioIds = anime.AnimeStudios.Select(ast => ast.StudioId).ToHashSet();

        var newStudios = new List<Domain.Entities.Studio>();
        foreach (var studioName in request.Studios)
        {
            var studio = await studioRepository.GetByNameAsync(studioName, ct)
                ?? throw new NotFoundException(AnimeErrors.StudioNotFoundInSystem(studioName));
            newStudios.Add(studio);
        }

        var newStudioIds = newStudios.Select(s => s.Id).ToHashSet();

        // Remove studios not in new list
        foreach (var ast in anime.AnimeStudios.Where(ast => !newStudioIds.Contains(ast.StudioId)).ToList())
            anime.RemoveStudio(ast.StudioId);

        // Add studios not in current list
        foreach (var studio in newStudios.Where(s => !currentStudioIds.Contains(s.Id)))
            anime.AddStudio(studio);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
