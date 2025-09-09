using WordCounterInterview.Models;
using WordCounterInterview.Services.Implementations;
using WordCounterInterview.Services.Interfaces;

namespace WordCounterInterview.Tests.Services;

/// <summary>
/// Test class for <see cref="IWordCountService"/>.
/// </summary>
public class WordCountServiceTests
{
    private readonly IWordCountService _service;

    /// <summary>
    /// Initializes the <see cref="IWordCountService"/> with
    /// an instance of <see cref="WordCountService"/>.
    /// </summary>
    public WordCountServiceTests()
    {
        _service = new WordCountService();
    }

    /// <summary>
    /// Unit test for <see cref="WordCountService.CountWords"/>.
    /// Ensures that when an empty string is provided as input,
    /// the service returns an empty collection of <see cref="WordCountResult"/>.
    /// </summary>
    [Fact]
    public void CountWords_EmptyContent_ReturnsEmpty()
    {
        // Arrange.
        string content = string.Empty;
        
        // Act.
        IEnumerable<WordCountResult> result = _service.CountWords(content);

        // Assert.
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that the service normalizes words to lowercase and counts correctly.
    /// </summary>
    [Fact]
    public void CountWords_NormalContent_ReturnsNormalizedCounts()
    {
        // Arrange.
        string content = "Hello hello world";

        // Act.
        List<WordCountResult> result = _service.CountWords(content).ToList();

        // Assert.
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Word == "hello" && r.Count == 2);
        Assert.Contains(result, r => r.Word == "world" && r.Count == 1);
    }

    /// <summary>
    /// Verifies that non-letter characters are removed.
    /// </summary>
    [Fact]
    public void CountWords_ContentWithNonLetters_RemovesNonLetters()
    {
        // Arrange.
        string content = "Test, test. 123 !";

        // Act.
        List<WordCountResult> result = _service.CountWords(content).ToList();

        // Assert.
        Assert.Single(result);
        Assert.Equal("test", result[0].Word);
        Assert.Equal(2, result[0].Count);
    }
    
    /// <summary>
    /// Verifies that multiple consecutive spaces, tabs, and newlines are collapsed into a single space.
    /// </summary>
    [Fact]
    public void CountWords_ContentWithMultipleWhitespace_CollapsesWhitespace()
    {
        // Arrange.
        string content = "Hello\t\tworld\nthis   is  a\ttest";

        // Act.
        List<WordCountResult> result = _service.CountWords(content).ToList();

        // Assert.
        Assert.Equal(6, result.Count);
        Assert.Contains(result, r => r.Word == "hello" && r.Count == 1);
        Assert.Contains(result, r => r.Word == "world"  && r.Count == 1);
        Assert.Contains(result, r => r.Word == "this" && r.Count == 1);
        Assert.Contains(result, r => r.Word == "is"  && r.Count == 1);
        Assert.Contains(result, r => r.Word == "a" && r.Count == 1);
        Assert.Contains(result, r => r.Word == "test" && r.Count == 1);
    }
    
    /// <summary>
    /// Verifies that words in different languages (Unicode) are handled correctly.
    /// </summary>
    [Fact]
    public void CountWords_ContentWithUnicode_HandlesUnicodeLetters()
    {
        // Arrange.
        string content = "Привет мир こんにちは 世界 Привет";

        // Act.
        List<WordCountResult> result = _service.CountWords(content).ToList();

        // Assert.
        Assert.Equal(4, result.Count);
        Assert.Contains(result, r => r.Word == "привет" && r.Count == 2);
        Assert.Contains(result, r => r.Word == "мир" && r.Count == 1);
        Assert.Contains(result, r => r.Word == "こんにちは" && r.Count == 1);
        Assert.Contains(result, r => r.Word == "世界" && r.Count == 1);
    }
    
    /// <summary>
    /// Verifies that multiple punctuation marks between words do not merge them.
    /// </summary>
    [Fact]
    public void CountWords_ContentWithPunctuation_DoesNotMergeWords()
    {
        // Arrange.
        string content = "Hello...world!!!";

        // Act.
        List<WordCountResult> result = _service.CountWords(content).ToList();

        // Assert.
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Word == "hello");
        Assert.Contains(result, r => r.Word == "world");
    }

}
