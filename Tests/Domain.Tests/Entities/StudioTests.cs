using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

public class StudioTests
{
    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var studio = new Studio("MAPPA", "Famous studio");

        Assert.NotEqual(Guid.Empty, studio.Id);
        Assert.Equal("MAPPA", studio.Name);
        Assert.Equal("Famous studio", studio.Description);
        Assert.True(studio.IsActive);
        Assert.Null(studio.LogoUrl);
        Assert.Null(studio.WebsiteUrl);
    }

    [Fact]
    public void Constructor_WithoutDescription_SetsDescriptionNull()
    {
        var studio = new Studio("MAPPA");

        Assert.Equal("MAPPA", studio.Name);
        Assert.Null(studio.Description);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsValidationException()
    {
        var act = () => new Studio(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateName_WithValidValue_UpdatesProperty()
    {
        var studio = new Studio("MAPPA");

        studio.UpdateName("ufotable");

        Assert.Equal("ufotable", studio.Name);
    }

    [Fact]
    public void UpdateName_WithNull_ThrowsValidationException()
    {
        var studio = new Studio("MAPPA");

        var act = () => studio.UpdateName(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_SetsDescription()
    {
        var studio = new Studio("MAPPA");

        studio.UpdateDescription("New description");

        Assert.Equal("New description", studio.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ClearsDescription()
    {
        var studio = new Studio("MAPPA", "Old desc");

        studio.UpdateDescription(null);

        Assert.Null(studio.Description);
    }

    [Fact]
    public void UpdateLogo_SetsLogoUrl()
    {
        var studio = new Studio("MAPPA");

        studio.UpdateLogo("https://example.com/logo.png");

        Assert.Equal("https://example.com/logo.png", studio.LogoUrl);
    }

    [Fact]
    public void UpdateLogo_WithNull_ClearsLogo()
    {
        var studio = new Studio("MAPPA");
        studio.UpdateLogo("https://example.com/logo.png");

        studio.UpdateLogo(null);

        Assert.Null(studio.LogoUrl);
    }
}
