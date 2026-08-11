using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class StudioNameTests
{
    [Fact]
    public void Create_WithValidName_ReturnsStudioName()
    {
        var name = StudioName.Create("MAPPA");

        Assert.NotNull(name);
        Assert.Equal("MAPPA", name.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var name = StudioName.Create("  MAPPA  ");

        Assert.Equal("MAPPA", name.Value);
    }

    [Fact]
    public void Create_WithNull_ThrowsValidationException()
    {
        var act = () => StudioName.Create(null);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsValidationException()
    {
        var act = () => StudioName.Create("");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsValidationException()
    {
        var act = () => StudioName.Create("   ");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithExceedingMaxLength_ThrowsValidationException()
    {
        var act = () => StudioName.Create(new string('a', StudioName.MaxLength + 1));

        Assert.Throws<ValidationException>(act);
    }
}
