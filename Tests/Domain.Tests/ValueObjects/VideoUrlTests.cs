using Domain.ValueObjects.AnimeObjects;
using Domain.Exceptions;

namespace Domain.Tests.ValueObjects;

public class VideoUrlTests
{
    [Theory]
    [InlineData("http://localhost:5100/api/upload/episode/x/stream")]
    [InlineData("https://cdn.example.com/video.m3u8")]
    public void Create_Succeeds_ForAbsoluteHttpUrl(string url)
    {
        var result = VideoUrl.Create(url);
        Assert.Equal(url, result.Value);
    }

    [Theory]
    [InlineData("hls/episode/media/playlist.m3u8")]
    [InlineData("")]
    public void Create_Throws_ForRelativeOrEmptyUrl(string url)
    {
        Assert.Throws<ValidationException>(() => VideoUrl.Create(url));
    }
}