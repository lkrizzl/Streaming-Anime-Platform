using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

public class GenreTests
{
    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var genre = new Genre("Action", "Action genre");

        Assert.NotEqual(Guid.Empty, genre.Id);
        Assert.Equal("Action", genre.Name);
        Assert.Equal("Action genre", genre.Description);
        Assert.True(genre.IsActive);
    }

    [Fact]
    public void Constructor_WithoutDescription_SetsDescriptionNull()
    {
        var genre = new Genre("Action");

        Assert.Equal("Action", genre.Name);
        Assert.Null(genre.Description);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsValidationException()
    {
        var act = () => new Genre(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateName_WithValidValue_UpdatesProperty()
    {
        var genre = new Genre("Action");

        genre.UpdateName("Comedy");

        Assert.Equal("Comedy", genre.Name);
    }

    [Fact]
    public void UpdateName_WithNull_ThrowsValidationException()
    {
        var genre = new Genre("Action");

        var act = () => genre.UpdateName(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_SetsDescription()
    {
        var genre = new Genre("Action");

        genre.UpdateDescription("New description");

        Assert.Equal("New description", genre.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ClearsDescription()
    {
        var genre = new Genre("Action", "Old desc");

        genre.UpdateDescription(null);

        Assert.Null(genre.Description);
    }

    [Fact]
    public void UpdateDescription_WithWhitespace_ClearsDescription()
    {
        var genre = new Genre("Action", "Old desc");

        genre.UpdateDescription("   ");

        Assert.Null(genre.Description);
    }
}
