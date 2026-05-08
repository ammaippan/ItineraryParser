using ItineraryParser.Core.Interfaces;
using ItineraryParser.Core.Models;
using ItineraryParser.Infrastructure.Interfaces;
using System.Text.Json;

namespace ItineraryParser.Infrastructure.Services;

/// <summary>
/// Handles transformation of unstructured travel text into structured itinerary data.
/// Uses an LLM for extraction and ensures the response is sanitized before parsing.
/// </summary>
public class ParserService
{
    private readonly ILLMService _llm;
    private readonly IPromptLoader _promptLoader;

    public ParserService(ILLMService llm, IPromptLoader promptLoader)
    {
        _llm = llm;
        _promptLoader = promptLoader;
    }

    /// <summary>
    /// Parses raw input text into a structured itinerary response.
    /// </summary>
    public async Task<ItineraryResponse?> ParseAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        // Build prompt from template and input
        var prompt = _promptLoader.Build(input);

        // Call LLM to extract structured data
        var rawResponse = await _llm.ExtractAsync(prompt);

        // Clean response (LLM may include extra text or markdown)
        var cleanedJson = ExtractJson(rawResponse);

        try
        {
            // Deserialize into strongly typed model
            var result =  JsonSerializer.Deserialize<ItineraryResponse>(
                cleanedJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result != null)
            {
                //Normalize output
                result.SourceCity = CapitalizeWords(result.SourceCity);
                result.Destination = CapitalizeWords(result.Destination);
            }

            return result;
        }
        catch
        {
            // If parsing fails, return null (handled in controller)
            return null;
        }
    }

    /// <summary>
    /// Extracts JSON block from LLM response.
    /// Handles cases where model returns extra text or markdown formatting.
    /// </summary>
    private static string ExtractJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Remove common markdown wrappers
        input = input.Replace("```json", "")
                     .Replace("```", "");

        var start = input.IndexOf('{');
        var end = input.LastIndexOf('}');

        if (start >= 0 && end > start)
        {
            return input.Substring(start, end - start + 1);
        }

        return input;
    }

    /// <summary>
    /// Converts a string into Title Case (e.g., "new york" → "New York").
    /// Ensures consistent formatting of city names.
    /// </summary>
    private static string? CapitalizeWords(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return string.Join(" ",
            value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }
}