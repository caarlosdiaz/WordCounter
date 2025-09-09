using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WordCounterInterview.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using WordCounterInterview.Models;

namespace WordCounterInterview.Tests.Integration.Controllers;

/// <summary>
/// Provides integration tests for the API endpoints exposed by 
/// <see cref="WordCountController"/>.
/// Uses <see cref="WebApplicationFactory{TEntryPoint}"/> to 
/// bootstrap the <see cref="Program"/> class and host the 
/// application in-memory for testing.
/// </summary>
public class WordCountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordCountControllerTests"/> class.
    /// Sets up the <see cref="HttpClient"/> using the provided 
    /// <see cref="WebApplicationFactory{Program}"/> to enable 
    /// integration testing of the API.
    /// </summary>
    /// <param name="factory">
    /// The <see cref="WebApplicationFactory{TEntryPoint}"/> that bootstraps 
    /// the <see cref="Program"/> application for testing.
    /// </param>
    public WordCountControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Uploads a valid UTF-8 text file and expects correct word counts.
    /// </summary>
    [Fact]
    public async Task UploadFile_ValidTxt_ReturnsWordCounts()
    {
        // Arrange.
        string text = "\nHello\n\n\n hello\t world world world!";
        using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(text));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", "test.txt");

        // Act.
        HttpResponseMessage response = await _client.PostAsync("/WordCount/file", form);

        // Assert.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<WordCountResult>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(results);
        Assert.Contains(results, r => r.Word == "hello" && r.Count == 2);
        Assert.Contains(results, r => r.Word == "world" && r.Count == 3);
    }

    /// <summary>
    /// Uploads an empty file and expects BadRequest.
    /// </summary>
    [Fact]
    public async Task UploadFile_EmptyFile_ReturnsBadRequest()
    {
        // Arrange.
        using var emptyContent = new ByteArrayContent(Array.Empty<byte>());
        emptyContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

        using var form = new MultipartFormDataContent();
        form.Add(emptyContent, "file", "empty.txt");

        // Act.
        HttpResponseMessage response = await _client.PostAsync("/WordCount/file", form);

        // Assert.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string message = await response.Content.ReadAsStringAsync();
        Assert.Contains("File is empty", message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uploads a file with disallowed extension and expects BadRequest.
    /// </summary>
    [Fact]
    public async Task UploadFile_DisallowedExtension_ReturnsBadRequest()
    {
        // Arrange.
        string text = "Some content";
        using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(text));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", "test.png");

        // Act.
        HttpResponseMessage response = await _client.PostAsync("/WordCount/file", form);

        // Assert.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string message = await response.Content.ReadAsStringAsync();
        Assert.Contains("Only .txt files are allowed", message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uploads a file exceeding max size and expects BadRequest.
    /// </summary>
    [Fact]
    public async Task UploadFile_FileTooLarge_ReturnsBadRequest()
    {
        // Arrange.
        byte[] largeFile = new byte[6 * 1024 * 1024];
        using var fileContent = new ByteArrayContent(largeFile);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", "bigfile.txt");

        // Act.
        HttpResponseMessage response = await _client.PostAsync("/WordCount/file", form);

        // Assert.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string message = await response.Content.ReadAsStringAsync();
        Assert.Contains("File size cannot exceed", message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uploads a file with invalid UTF-8 encoding and expects BadRequest.
    /// </summary>
    [Fact]
    public async Task UploadFile_InvalidEncoding_ReturnsBadRequest()
    {
        // Arrange.
        byte[] isoBytes = [0xC0, 0xAF];;
        using var fileContent = new ByteArrayContent(isoBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", "wrongEncoding.txt");

        // Act.
        HttpResponseMessage response = await _client.PostAsync("/WordCount/file", form);

        // Assert.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string message = await response.Content.ReadAsStringAsync();
        Assert.Contains("not valid UTF-8", message, StringComparison.OrdinalIgnoreCase);
    }
    
}