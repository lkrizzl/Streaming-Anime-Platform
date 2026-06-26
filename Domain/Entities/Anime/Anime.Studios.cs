using Domain.Associations;
using Domain.Exceptions;
using System.Linq;

namespace Domain.Entities;

// ====================== Управління студіями ======================
public partial class Anime
{
    public void AddStudio(Studio studio)
    {
        if (studio == null) throw new ValidationException("Studio cannot be null");

        if (AnimeStudios.Any(ast => ast.StudioId == studio.Id))
            return;

        AnimeStudios.Add(new AnimeStudio(Id, studio.Id));
        UpdatedOnUtc = UtcNow;
    }

    public void RemoveStudio(Guid studioId)
    {
        var ast = AnimeStudios.FirstOrDefault(x => x.StudioId == studioId);
        if (ast is not null)
        {
            AnimeStudios.Remove(ast);
            UpdatedOnUtc = UtcNow;
        }
    }
}
