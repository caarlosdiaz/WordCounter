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
    /// Regex explanation for text normalization:
    /// - \p{L} matches any kind of letter in Unicode (Latin, Cyrillic, Greek, Asian, etc.).
    /// - \s matches any whitespace character (spaces, tabs, line breaks).
    /// - [^\p{L}\s] means "any character that is not a letter or a whitespace".
    /// These characters are replaced by a space to avoid concatenating words accidentally.
    /// - \s+ matches one or more consecutive whitespace characters.
    /// After replacement, multiple consecutive spaces are collapsed into a single space.
    /// </remarks>
    private string NormalizeContent(string content)
    {
        string loweredContent = content.ToLowerInvariant();
        
        string lettersOnlyContent = Regex.Replace(loweredContent, @"[^\p{L}\s]", " ");
        
        return Regex.Replace(lettersOnlyContent, @"\s+", " ").Trim();
    }
}