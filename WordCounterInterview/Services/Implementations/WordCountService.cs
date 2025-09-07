using System.Text.RegularExpressions;
using WordCounterInterview.Models;
using WordCounterInterview.Services.Interfaces;

namespace WordCounterInterview.Services.Implementations;

/// <inheritdoc />
public class WordCountService : IWordCountService
{
    /// <inheritdoc />
    public IEnumerable<WordCountResult> CountWords(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return Enumerable.Empty<WordCountResult>();
        }
        
        string normalizedContent = NormalizeContent(content);
        
        return normalizedContent
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(w => w)
            .Select(w => new WordCountResult()
            {
                Word = w.Key,
                Count = w.Count()
            });
    }
    
    /// <summary>
    /// Normalizes the text: lowercases, removes non-letter characters.
    /// </summary>
    /// <remarks>
    /// Regex explanation:
    /// - \p{L} matches any kind of letter in Unicode (Latin, Cyrillic, Greek, Asian, etc.).
    /// - \s matches any whitespace character (spaces, tabs, line breaks).
    /// - [^\p{L}\s] means "any character that is not a letter or a whitespace".
    /// This ensures the text only contains valid words separated by spaces.
    /// </remarks>
    private string NormalizeContent(string content)
    {
        string loweredContent = content.ToLowerInvariant();
        return Regex.Replace(loweredContent, @"[^\p{L}\s]", "");
    }
}