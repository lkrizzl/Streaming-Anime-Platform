using Domain.Entities;

namespace Application.Abstractions;

public record AnimeFilter(
    string? Search = null,
    string? Genre = null,
    AnimeStatus? Status = null,
    string? SortBy = "created",
    string? SortOrder = "desc");
