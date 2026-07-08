using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ReturnsPassword()
    {
        var password = Password.Create("StrongPass1");

        Assert.NotNull(password);
        Assert.Equal("StrongPass1", password.Value);
    }

    [Fact]
    public void Create_WithMinLengthPassword_ReturnsPassword()
    {
        var password = Password.Create("abc12345");

        Assert.Equal(8, password.Value.Length);
        Assert.NotNull(password);
    }

    [Fact]
    public void Create_WithMaxLengthPassword_ReturnsPassword()
    {
        var password = Password.Create("a1" + new string('x', 62));

        Assert.Equal(64, password.Value.Length);
        Assert.NotNull(password);
    }

    [Fact]
    public void Create_WithNull_ThrowsEntityValidationException()
    {
        var act = () => Password.Create(null);

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithTooShortPassword_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("Ab1");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithTooLongPassword_ThrowsEntityValidationException()
    {
        var act = () => Password.Create(new string('a', 65) + "1A");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithoutLetters_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("12345678");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithoutDigits_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("abcdefgh");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithOnlyLetters_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("abcdefgh");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithOnlyDigits_ThrowsEntityValidationException()
    {
        var act = () => Password.Create("12345678");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var password = Password.Create("ValidPass1");

        string value = password;

        Assert.Equal("ValidPass1", value);
    }
}
