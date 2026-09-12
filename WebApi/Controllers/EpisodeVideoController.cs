using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers;

public class MediaServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string InternalApiKey { get; set; } = string.Empty;
}

[ApiController]
[Route("api/episodes")]
public class EpisodeVideoController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly MediaServiceOptions _options;

    public EpisodeVideoController(HttpClient httpClient, IOptions<MediaServiceOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{episodeId:guid}/video")]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> UploadVideoAsync(Guid episodeId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is empty.");

        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

        content.Add(streamContent, "file", file.FileName);
        content.Add(new StringContent(episodeId.ToString()), "episodeId");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/api/upload")
        {
            Content = content
        };
        request.Headers.Add("X-Internal-Api-Key", _options.InternalApiKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, body);
    }
}