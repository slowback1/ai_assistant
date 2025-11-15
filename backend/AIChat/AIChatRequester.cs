using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIChat;

public class AIChatRequester : IAIRequester
{
	private readonly HttpClient? _httpClient;

	public AIChatRequester()
	{
		_httpClient = null;
	}

	// Allow injecting a pre-configured HttpClient (useful for tests)
	public AIChatRequester(HttpClient httpClient)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
	}

	public AIResult GetAIResponse(AIContext context)
	{
		if (context == null) throw new ArgumentNullException(nameof(context));
		if (context.Config == null)
			throw new ArgumentException("AIConfig is required on the context.", nameof(context));
		if (string.IsNullOrWhiteSpace(context.Config.Url))
			throw new ArgumentException("AIConfig.Url must be provided.", nameof(context));

		var client = _httpClient ?? new HttpClient();
		var disposeClient = _httpClient == null;
		try
		{
			// add API key header if present
			if (!string.IsNullOrWhiteSpace(context.Config.ApiKey))
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", context.Config.ApiKey);

			var payload = new
			{
				model = context.Config.Model ?? "deepseek-r1",
				prompt = context.Message ?? string.Empty,
				sessionId = context.SessionId,
				stream = false,
				think = false
			};

			var json = JsonConvert.SerializeObject(payload);
			using var content = new StringContent(json, Encoding.UTF8, "application/json");

			var resp = client.PostAsync(context.Config.Url, content).GetAwaiter().GetResult();
			var respBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

			string responseText;
			try
			{
				var token = JToken.Parse(respBody);
				if (token.Type == JTokenType.Object)
				{
					var obj = (JObject)token;
					if (obj.TryGetValue("response", StringComparison.OrdinalIgnoreCase, out var r) &&
					    r.Type == JTokenType.String)
					{
						responseText = r.Value<string>() ?? respBody;
					}
					else if (obj.TryGetValue("output", StringComparison.OrdinalIgnoreCase, out var o) &&
					         o.Type == JTokenType.String)
					{
						responseText = o.Value<string>() ?? respBody;
					}
					else
					{
						// fallback: try to find the first string token
						var firstString = obj.Descendants().OfType<JValue>()
							.FirstOrDefault(v => v.Type == JTokenType.String)?.Value as string;
						responseText = firstString ?? respBody;
					}
				}
				else
				{
					responseText = respBody;
				}
			}
			catch
			{
				responseText = respBody;
			}

			return new AIResult
			{
				Response = responseText,
				SessionId = context.SessionId ?? Guid.NewGuid().ToString()
			};
		}
		catch (Exception ex)
		{
			return new AIResult
			{
				Response = $"Error: {ex.Message}",
				SessionId = context.SessionId ?? Guid.NewGuid().ToString()
			};
		}
		finally
		{
			if (disposeClient) client.Dispose();
		}
	}
}