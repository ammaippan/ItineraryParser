using ItineraryParser.Infrastructure.Interfaces;

namespace ItineraryParser.Infrastructure.Services;

/// <summary>
/// Loads and builds the LLM prompt from a template file.
/// Keeps prompt externalized for easier maintenance and updates.
/// </summary>
public class PromptLoader : IPromptLoader
{
    private readonly string _template;

    public PromptLoader()
    {
        // Resolve prompt file path from application base directory
        var path = Path.Combine(AppContext.BaseDirectory, "Prompts", "itinerary_prompt.txt");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Prompt file not found", path);
        }

        // Load once (avoid reading file on every request)
        _template = File.ReadAllText(path);
    }

    /// <summary>
    /// Injects user input into the prompt template.
    /// </summary>
    public string Build(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty", nameof(input));

        return _template.Replace("{input}", input);
    }
}