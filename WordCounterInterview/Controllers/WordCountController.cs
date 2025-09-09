using System.Text;
using Microsoft.AspNetCore.Mvc;
using WordCounterInterview.Models;
using WordCounterInterview.Services.Interfaces;

namespace WordCounterInterview.Controllers;

/// <summary>
/// Provides API endpoints for word count operations.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WordCountController : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".txt"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private static readonly Encoding FileEncoding = new UTF8Encoding(false, true);

    private readonly ILogger<WordCountController> _logger;
    private readonly IWordCountService  _wordCountService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordCountController"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger instance used to record diagnostic and error information 
    /// within the controller.
    /// </param>
    /// <param name="wordCountService">
    /// The <see cref="IWordCountService"/> used to count the occurrences
    /// of each unique word in the provided content
    /// </param>
    public WordCountController(
        ILogger<WordCountController> logger,  
        IWordCountService wordCountService)
    {
        this._logger = logger;
        this._wordCountService = wordCountService;
    }
    
    /// <summary>
    /// Uploads a text file and returns frequency of each
    /// unique word in the provided text.
    /// </summary>
    /// <param name="file">
    /// The uploaded text file to analyze. 
    /// Must meet the following requirements:
    /// - Extension: .txt
    /// - Maximum size: 5 MB
    /// - Encoding: UTF-8
    /// </param>
    /// <returns>
    /// A <see cref="IEnumerable{WordCountResult}"/>  representing
    /// the frequency of each unique word extracted from the file content.
    /// </returns>
    [HttpPost("file")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IEnumerable<WordCountResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            ValidateFile(file);
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, "File validation failed in file endpoint.");
            
            return BadRequest(e.Message);
        }

        string content;
        
        try
        {
            using StreamReader reader = new StreamReader(file.OpenReadStream(),  FileEncoding);
            content = await reader.ReadToEndAsync();
        }
        catch (DecoderFallbackException e)
        {
            _logger.LogError(e, "Failed to read the uploaded file as UTF-8.");
    
            return BadRequest("The file is not valid UTF-8 encoded text.");
        }
        
        IEnumerable<WordCountResult> results = _wordCountService.CountWords(content);

        return Ok(results);
    }

    /// <summary>
    /// Validates that the input file:
    /// - Is not empty.
    /// - The extensions is .txt.
    /// - The size in MB does not exceed 5MB.
    /// </summary>
    /// <param name="file">The uploaded text file to analyze. </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the file is null, empty, has an unsupported extension,
    /// exceeds the maximum allowed size, or cannot be read with UTF-8 encoding.
    /// </exception>
    private void ValidateFile(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new ArgumentException("File is empty.");
        }

        string? extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Only .txt files are allowed.");
            
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ArgumentException("File size cannot exceed 5 MB.");
            
        }
    }
}