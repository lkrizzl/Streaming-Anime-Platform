using Domain.Exceptions;
using System.Linq;

namespace Domain.Entities;

public partial class Anime
{
    public void AddStudio(Studio studio)
    {
        //if (studio == null) throw new ValidationException("Studio cannot be null");

        if (AnimeStudios.Any(ast => ast.StudioId == studio.Id))
            return;

        _animeStudios.Add(new AnimeStudio(Id, studio.Id));
        UpdatedOnUtc = UtcNow;
    }

    public void RemoveStudio(Guid studioId)
    {
        var ast = AnimeStudios.FirstOrDefault(x => x.StudioId == studioId);
        if (ast is not null)
        {
            _animeStudios.Remove(ast);
            UpdatedOnUtc = UtcNow;
        }
    }
}
