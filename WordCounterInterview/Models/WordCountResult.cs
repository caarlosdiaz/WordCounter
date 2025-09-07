namespace WordCounterInterview.Models;

/// <summary>
/// Represents a unique word and its occurrences in a text.
/// </summary>
public class WordCountResult
{
   /// <summary>
   /// The unique word.
   /// </summary>
   public string Word { get; set; } = string.Empty;
   
   /// <summary>
   /// The occurrences of the unique word in a text. 
   /// </summary>
   public int Count { get; set; }
}