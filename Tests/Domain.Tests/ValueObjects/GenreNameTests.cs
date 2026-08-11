using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class GenreNameTests
{
    [Fact]
    public void Create_WithValidName_ReturnsGenreName()
    {
        var name = GenreName.Create("Action");

        Assert.NotNull(name);
        Assert.Equal("Action", name.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var name = GenreName.Create("  Action  ");

        Assert.Equal("Action", name.Value);
    }

    [Fact]
    public void Create_WithNull_ThrowsValidationException()
    {
        var act = () => GenreName.Create(null);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsValidationException()
    {
        var act = () => GenreName.Create("");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsValidationException()
    {
        var act = () => GenreName.Create("   ");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithExceedingMaxLength_ThrowsValidationException()
    {
        var act = () => GenreName.Create(new string('a', GenreName.MaxLength + 1));

        Assert.Throws<ValidationException>(act);
    }
}
