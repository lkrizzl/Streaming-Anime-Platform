using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class GenreTests
{
    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var genre = new Genre(GenreName.Create("Action"), "Action genre");

        Assert.NotEqual(Guid.Empty, genre.Id);
        Assert.Equal("Action", genre.Name);
        Assert.Equal("Action genre", genre.Description);
        Assert.True(genre.IsActive);
    }

    [Fact]
    public void Constructor_WithoutDescription_SetsDescriptionNull()
    {
        var genre = new Genre(GenreName.Create("Action"));

        Assert.Equal("Action", genre.Name);
        Assert.Null(genre.Description);
    }

    [Fact]
    public void UpdateName_WithValidValue_UpdatesProperty()
    {
        var genre = new Genre(GenreName.Create("Action"));

        genre.UpdateName(GenreName.Create("Comedy"));

        Assert.Equal("Comedy", genre.Name);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_SetsDescription()
    {
        var genre = new Genre(GenreName.Create("Action"));

        genre.UpdateDescription("New description");

        Assert.Equal("New description", genre.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ClearsDescription()
    {
        var genre = new Genre(GenreName.Create("Action"), "Old desc");

        genre.UpdateDescription(null);

        Assert.Null(genre.Description);
    }

    [Fact]
    public void UpdateDescription_WithWhitespace_ClearsDescription()
    {
        var genre = new Genre(GenreName.Create("Action"), "Old desc");

        genre.UpdateDescription("   ");

        Assert.Null(genre.Description);
    }
}
