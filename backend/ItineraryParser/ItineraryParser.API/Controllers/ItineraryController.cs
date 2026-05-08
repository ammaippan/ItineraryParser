using ItineraryParser.Core.Models;
using ItineraryParser.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItineraryParser.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItineraryController : ControllerBase
{
    private readonly ParserService _parser;
    private readonly ILogger<ItineraryController> _logger;

    public ItineraryController(
        ParserService parser,
        ILogger<ItineraryController> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    [HttpPost("parse")]
    public async Task<IActionResult> Parse([FromBody] ItineraryRequest request)
    {
        _logger.LogInformation("Received itinerary parsing request");

        if (request == null || string.IsNullOrWhiteSpace(request.Text))
        {
            _logger.LogWarning("Invalid request: empty input text");
            return BadRequest(new { error = "Input text is required" });
        }

        var result = await _parser.ParseAsync(request.Text);

        // Parsing failed (LLM issue or invalid JSON)
        if (result == null)
        {
            _logger.LogWarning("Parsing failed: invalid response from AI");
            return BadRequest(new { error = "Failed to parse itinerary" });
        }

        // Validation: required fields
        if (string.IsNullOrWhiteSpace(result.SourceCity) ||
            string.IsNullOrWhiteSpace(result.Destination))
        {
            _logger.LogWarning("Validation failed: missing source or destination");
            return BadRequest(new
            {
                error = "Missing required travel information",
                details = "SourceCity and Destination are required"
            });
        }

        // Handle edge case where LLM defaults to 2024 for dates
        if (result.StartDate != null && result.StartDate.StartsWith("2024"))
        {
            var currentYear = DateTime.UtcNow.Year;
            result.StartDate = result.StartDate.Replace("2024", currentYear.ToString());
        }
        // Handle edge case where LLM defaults to 2024 for dates
        if (result.EndDate != null && result.EndDate.StartsWith("2024"))
        {
            var currentYear = DateTime.UtcNow.Year;
            result.EndDate = result.EndDate.Replace("2024", currentYear.ToString());
        }
        _logger.LogInformation("Itinerary parsed successfully");
        return Ok(result);
    }
}