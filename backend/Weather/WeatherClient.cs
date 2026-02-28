using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Weather;

public class WeatherClient
{
    private readonly WeatherConfig _config;
    private readonly HttpClient _httpClient;

    public WeatherClient(WeatherConfig config, HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<WeatherDto> GetCurrentWeatherAsync()
    {
        var url = $"https://api.weatherapi.com/v1/forecast.json?key={_config.ApiKey}&q={_config.ZipCode}&days=1&aqi=no&alerts=no";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new WeatherApiResponse();

        var icon = apiResponse.Current.Condition.Icon.TrimStart('/');

        return new WeatherDto
        {
            Name = apiResponse.Location.Name,
            TempF = apiResponse.Current.TempF,
            WindMph = apiResponse.Current.WindMph,
            PrecipIn = apiResponse.Current.PrecipIn,
            HeatindexF = apiResponse.Current.HeatindexF,
            Condition = apiResponse.Current.Condition.Text,
            ConditionIcon = icon
        };
    }
}
