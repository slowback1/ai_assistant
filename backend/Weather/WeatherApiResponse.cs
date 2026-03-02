using System.Text.Json.Serialization;

namespace Weather;

public class WeatherApiResponse
{
    [JsonPropertyName("location")]
    public WeatherLocation Location { get; set; } = new();

    [JsonPropertyName("current")]
    public WeatherCurrent Current { get; set; } = new();
}

public class WeatherLocation
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class WeatherCurrent
{
    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }

    [JsonPropertyName("wind_mph")]
    public double WindMph { get; set; }

    [JsonPropertyName("precip_in")]
    public double PrecipIn { get; set; }

    [JsonPropertyName("heatindex_f")]
    public double HeatindexF { get; set; }

    [JsonPropertyName("condition")]
    public WeatherCondition Condition { get; set; } = new();
}

public class WeatherCondition
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}
