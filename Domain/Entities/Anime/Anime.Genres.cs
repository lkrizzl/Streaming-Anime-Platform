using Domain.Associations;
using Domain.Exceptions;
using System.Linq;

namespace Domain.Entities;

// ====================== Управління жанрами ======================
public partial class Anime
{
    public void AddGenre(Genre genre)
    {
        if (genre == null) throw new ValidationException("Genre cannot be null");

        if (AnimeGenres.Any(ag => ag.GenreId == genre.Id))
            return;

        AnimeGenres.Add(new AnimeGenre(Id, genre.Id));
        UpdatedOnUtc = UtcNow;
    }

    public void RemoveGenre(Guid genreId)
    {
        var ag = AnimeGenres.FirstOrDefault(x => x.GenreId == genreId);
        if (ag is not null)
        {
            AnimeGenres.Remove(ag);
            UpdatedOnUtc = UtcNow;
        }
    }
}
