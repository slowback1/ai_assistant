using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weather.Tests;

public class WeatherClientTests
{
    private const string SampleResponse = @"{
        ""location"": {
            ""name"": ""San Diego""
        },
        ""current"": {
            ""temp_f"": 30.0,
            ""wind_mph"": 10.5,
            ""precip_in"": 0.0,
            ""heatindex_f"": 33.0,
            ""condition"": {
                ""text"": ""Cloudy"",
                ""icon"": ""//cdn.weatherapi.com/weather/64x64/night/119.png""
            }
        }
    }";

    [Fact]
    public async Task GetCurrentWeatherAsync_MapsResponseToDto()
    {
        var handler = new FakeHttpMessageHandler(req =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(SampleResponse)
            });

        using var client = new HttpClient(handler);
        var config = new WeatherConfig { ApiKey = "test-key", ZipCode = "12345" };
        var weatherClient = new WeatherClient(config, client);

        var result = await weatherClient.GetCurrentWeatherAsync();

        Assert.NotNull(result);
        Assert.Equal("San Diego", result.Name);
        Assert.Equal(30.0, result.TempF);
        Assert.Equal(10.5, result.WindMph);
        Assert.Equal(0.0, result.PrecipIn);
        Assert.Equal(33.0, result.HeatindexF);
        Assert.Equal("Cloudy", result.Condition);
        Assert.Equal("cdn.weatherapi.com/weather/64x64/night/119.png", result.ConditionIcon);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_UsesApiKeyAndZipCodeInUrl()
    {
        string? capturedUrl = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(SampleResponse)
            };
        });

        using var client = new HttpClient(handler);
        var config = new WeatherConfig { ApiKey = "my-api-key", ZipCode = "90210" };
        var weatherClient = new WeatherClient(config, client);

        await weatherClient.GetCurrentWeatherAsync();

        Assert.NotNull(capturedUrl);
        Assert.Contains("my-api-key", capturedUrl);
        Assert.Contains("90210", capturedUrl);
    }

    [Fact]
    public void Constructor_ThrowsOnNullConfig()
    {
        Assert.Throws<ArgumentNullException>(() => new WeatherClient(null!));
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var resp = _responder(request);
            return Task.FromResult(resp);
        }
    }
}
