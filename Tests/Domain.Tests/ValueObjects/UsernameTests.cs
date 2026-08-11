using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class UsernameTests
{
    [Fact]
    public void Create_WithValidUsername_ReturnsUsername()
    {
        var username = Username.Create("john_doe");

        Assert.NotNull(username);
        Assert.Equal("john_doe", username.Value);
    }

    [Fact]
    public void Create_WithAlphanumericUsername_ReturnsUsername()
    {
        var username = Username.Create("JohnDoe123");

        Assert.Equal("JohnDoe123", username.Value);
    }

    [Fact]
    public void Create_WithHyphensAndDots_ReturnsUsername()
    {
        var username = Username.Create("john.doe-test");

        Assert.Equal("john.doe-test", username.Value);
    }

    [Fact]
    public void Create_WithNull_ThrowsValidationException()
    {
        var act = () => Username.Create(null);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsValidationException()
    {
        var act = () => Username.Create("");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithTooShortUsername_ThrowsValidationException()
    {
        var act = () => Username.Create("ab");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithTooLongUsername_ThrowsValidationException()
    {
        var act = () => Username.Create(new string('a', 31));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithInvalidCharacters_ThrowsValidationException()
    {
        var act = () => Username.Create("user name!");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithLeadingSpecialChar_ThrowsValidationException()
    {
        var act = () => Username.Create(".john");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Create_WithTrailingSpecialChar_ThrowsValidationException()
    {
        var act = () => Username.Create("john.");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var username = Username.Create("john_doe");

        string value = username;

        Assert.Equal("john_doe", value);
    }
}
