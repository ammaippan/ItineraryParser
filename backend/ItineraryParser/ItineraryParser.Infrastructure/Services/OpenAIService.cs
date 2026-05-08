using OpenAI.Chat;
using ItineraryParser.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItineraryParser.Infrastructure.Services;

/// <summary>
/// Calls OpenAI to extract structured data from unstructured text.
/// Assumes the prompt enforces strict JSON, but still guards against empty/invalid responses.
/// </summary>
public class OpenAIService : ILLMService
{
    private readonly ChatClient _client;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(string apiKey, ILogger<OpenAIService> logger)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required", nameof(apiKey));

        _client = new ChatClient("gpt-4.1-mini", apiKey);
        _logger = logger;
    }

    public async Task<string> ExtractAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

        try
        {
            var response = await _client.CompleteChatAsync(prompt);

            // LLM responses can be inconsistent; guard against empty content
            var content = response?.Value?.Content?.FirstOrDefault()?.Text;

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("Empty response from OpenAI");

            return content;
        }
        catch (Exception ex)
        {
            // Log once at integration boundary; let global middleware shape the HTTP response
            _logger.LogError(ex, "Error calling OpenAI");
            throw;
        }
    }
}