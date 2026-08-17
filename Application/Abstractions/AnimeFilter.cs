using Domain.Entities;

namespace Application.Abstractions;

public record AnimeFilter(
    string? Search = null,
    string? Genre = null,
    AnimeStatus? Status = null,
    int? ReleaseYear = null,
    string? Studio = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? SortBy = "created",
    string? SortOrder = "desc");
