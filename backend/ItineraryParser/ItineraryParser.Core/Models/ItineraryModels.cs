
using System.Text.Json.Serialization;

namespace ItineraryParser.Core.Models;

public class ItineraryRequest
{
    public string Text { get; set; }
}


public class ItineraryResponse
{
    [JsonPropertyName("SourceCity")]
    public string? SourceCity { get; set; }

    [JsonPropertyName("Destination")]
    public string? Destination { get; set; }

    [JsonPropertyName("StartDate")]
    public string? StartDate { get; set; }

    [JsonPropertyName("EndDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("Adults")]
    public int? Adults { get; set; }

    [JsonPropertyName("Children")]
    public int? Children { get; set; }

    [JsonPropertyName("Budget")]
    public decimal? Budget { get; set; }

    [JsonPropertyName("Currency")]
    public string? Currency { get; set; }
}