using WordCounterInterview.Models;

namespace WordCounterInterview.Services.Interfaces;

/// <summary>
/// Provides functionality to count the occurrences of each
/// unique word in the provided content.
/// </summary>
public interface IWordCountService
{
    /// <summary>
    /// Counts the frequency of each unique word in the provided text removing
    /// non-letter characters and collapsing multiple spaces.
    /// </summary>
    /// <param name="content">The input text to analyze. Must not be null or empty.</param>
    /// <returns>
    /// A collection of <see cref="WordCountResult"/> representing each unique word 
    /// and its corresponding count.
    /// </returns>
    IEnumerable<WordCountResult> CountWords(string content); 
}