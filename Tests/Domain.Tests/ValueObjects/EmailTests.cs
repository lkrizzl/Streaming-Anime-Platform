using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ReturnsEmail()
    {
        var email = Email.Create("test@example.com");

        Assert.NotNull(email);
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_TrimsAndLowercasesEmail()
    {
        var email = Email.Create("  Test@Example.COM  ");

        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_WithNull_ThrowsEntityValidationException()
    {
        var act = () => Email.Create(null);

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsEntityValidationException()
    {
        var act = () => Email.Create("");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsEntityValidationException()
    {
        var act = () => Email.Create("   ");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithTooLongEmail_ThrowsEntityValidationException()
    {
        var localPart = new string('a', 250);
        var longEmail = $"{localPart}@b.com";

        var act = () => Email.Create(longEmail);

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithoutAtSymbol_ThrowsEntityValidationException()
    {
        var act = () => Email.Create("invalid-email");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void Create_WithoutDomain_ThrowsEntityValidationException()
    {
        var act = () => Email.Create("user@");

        Assert.Throws<EntityValidationException>(act);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var email = Email.Create("test@example.com");

        string value = email;

        Assert.Equal("test@example.com", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var email = Email.Create("test@example.com");

        Assert.Equal("test@example.com", email.ToString());
    }
}
