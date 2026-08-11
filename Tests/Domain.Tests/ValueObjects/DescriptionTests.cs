using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class DescriptionTests
{
    private const int MaxLength = 500;

    [Fact]
    public void Create_WithValidText_ReturnsDescription()
    {
        var description = Description.Create("Test description", MaxLength);

        Assert.NotNull(description);
        Assert.Equal("Test description", description.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var description = Description.Create("  Test description  ", MaxLength);

        Assert.Equal("Test description", description.Value);
    }

    [Fact]
    public void Create_AtMaxLength_ReturnsDescription()
    {
        var text = new string('a', MaxLength);

        var description = Description.Create(text, MaxLength);

        Assert.Equal(text, description.Value);
    }

    [Fact]
    public void Create_WithNull_ThrowsValidationException()
    {
        var act = () => Description.Create(null, MaxLength);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsValidationException()
    {
        var act = () => Description.Create("", MaxLength);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsValidationException()
    {
        var act = () => Description.Create("   ", MaxLength);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithExceedingMaxLength_ThrowsValidationException()
    {
        var act = () => Description.Create(new string('a', MaxLength + 1), MaxLength);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var description = Description.Create("Test description", MaxLength);

        string value = description;

        Assert.Equal("Test description", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var description = Description.Create("Test description", MaxLength);

        Assert.Equal("Test description", description.ToString());
    }
}
