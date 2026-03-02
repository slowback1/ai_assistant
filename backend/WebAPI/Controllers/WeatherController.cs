using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Weather;

namespace WebAPI.Controllers;

[Route("Weather")]
public class WeatherController : ApplicationController
{
    private readonly WeatherClient? _weatherClient;

    public WeatherController(ICrudFactory crudFactory, WeatherClient? weatherClient = null) : base(crudFactory)
    {
        _weatherClient = weatherClient;
    }

    [HttpGet("Current")]
    public async Task<ActionResult> GetCurrentWeather()
    {
        if (_weatherClient == null)
        {
            return BadRequest(new { error = "Weather integration is not configured" });
        }

        try
        {
            var weather = await _weatherClient.GetCurrentWeatherAsync();
            return Ok(weather);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch weather data", details = ex.Message });
        }
    }
}
