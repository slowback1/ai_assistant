using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Donetick;

public class DonetickClient
{
    private readonly DonetickConfig _config;
    private readonly HttpClient _httpClient;

    public DonetickClient(DonetickConfig config, HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<List<ChoreDto>> GetAllChoresAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.InstanceUrl}/chore");
        request.Headers.Add("secretkey", _config.ApiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var chores = JsonSerializer.Deserialize<List<ChoreDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return chores ?? new List<ChoreDto>();
    }

    public async Task<bool> CompleteChoreAsync(int choreId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.InstanceUrl}/chore/{choreId}/complete");
        request.Headers.Add("secretkey", _config.ApiKey);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
