using System.Net;
using Common.Models;

namespace AIChat.Tests;

public class AIChatRequesterTests
{
	[Test]
	public void GetAIResponse_ParsesResponseField()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{ \"response\": \"hello world\" }")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
		{
			Config = new AIConfig { Url = "https://test.local" }, Message = "hi",
			Session = new AISession { SessionId = "s1" }
		};
		var result = requester.GetAIResponse(ctx);

		Assert.That(result.Response, Is.EqualTo("hello world"));
		Assert.That(result.SessionId, Is.EqualTo("s1"));
	}

	[Test]
	public void GetAIResponse_ParsesOutputField()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{ \"output\": \"from output\" }")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
		{
			Config = new AIConfig { Url = "https://test.local" }, Message = "hi",
			Session = new AISession { SessionId = "s2" }
		};
		var result = requester.GetAIResponse(ctx);

		Assert.That(result.Response, Is.EqualTo("from output"));
		Assert.That(result.SessionId, Is.EqualTo("s2"));
	}

	[Test]
	public void GetAIResponse_NonJsonBody_FallsBackToRaw()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("plain text result")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
		{
			Config = new AIConfig { Url = "https://test.local" }, Message = "hi",
			Session = new AISession { SessionId = "s3" }
		};
		var result = requester.GetAIResponse(ctx);

		Assert.That(result.Response, Is.EqualTo("plain text result"));
		Assert.That(result.SessionId, Is.EqualTo("s3"));
	}

	[Test]
	public void GetAIResponse_IncludesAuthorizationHeaderWhenApiKeySet()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			// Inspect request headers and return a simple response
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{ \"response\": \"ok\" }")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
		{
			Config = new AIConfig { Url = "https://test.local", ApiKey = "my-secret" }, Message = "hi",
			Session = new AISession { SessionId = "s5" }
		};
		var result = requester.GetAIResponse(ctx);

		// ensure handler captured the request and the Authorization header was set
		Assert.That(handler.LastRequest, Is.Not.Null);
		var auth = handler.LastRequest!.Headers.Authorization;
		Assert.That(auth, Is.Not.Null);
		Assert.That(auth!.Scheme, Is.EqualTo("Bearer"));
		Assert.That(auth.Parameter, Is.EqualTo("my-secret"));
		Assert.That(result.Response, Is.EqualTo("ok"));
	}

	[Test]
	public void GetAIResponse_ThrowsOnMissingUrl()
	{
		var requester = new AIChatRequester();

		var ctx = new AIContext { Config = new AIConfig { Url = "" }, Message = "hi" };

		var ex = Assert.Throws<ArgumentException>(() => requester.GetAIResponse(ctx));
		Assert.That(ex!.ParamName, Is.EqualTo("context"));
	}

	[Test]
	public void GetAIResponse_ThrowsOnNullContext()
	{
		var requester = new AIChatRequester();

		var ex = Assert.Throws<ArgumentNullException>(() => requester.GetAIResponse(null!));
		Assert.That(ex!.ParamName, Is.EqualTo("context"));
	}

	[Test]
	public void GetAIResponse_ThrowsOnNullConfig()
	{
		var requester = new AIChatRequester();

		var ctx = new AIContext { Config = null!, Message = "hi" };

		var ex = Assert.Throws<ArgumentException>(() => requester.GetAIResponse(ctx));
		Assert.That(ex!.ParamName, Is.EqualTo("context"));
	}

	[Test]
	public void GetAIResponse_UsesDefaultModelWhenNotSet()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{ \"response\": \"default model used\" }")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
		{
			Config = new AIConfig { Url = "https://test.local", Model = null! }, Message = "hi",
			Session = new AISession { SessionId = "s5" }
		};
		var result = requester.GetAIResponse(ctx);

		Assert.That(result.Response, Is.EqualTo("default model used"));
		Assert.That(result.SessionId, Is.EqualTo("s5"));
	}

	[Test]
	public void GetAIResponse_GeneratesAGuidForTheSessionIdWhenNotSet()
	{
		var handler = new FakeHttpMessageHandler(req =>
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{ \"response\": \"no session id\" }")
			};
			return msg;
		});

		using var client = new HttpClient(handler);
		var requester = new AIChatRequester(client);

		var ctx = new AIContext
			{ Config = new AIConfig { Url = "https://test.local" }, Message = "hi", Session = null! };
		var result = requester.GetAIResponse(ctx);

		Assert.That(result.Response, Is.EqualTo("no session id"));
		Assert.That(Guid.TryParse(result.SessionId, out _), Is.True);
	}

	private class FakeHttpMessageHandler : HttpMessageHandler
	{
		private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

		public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
		{
			_responder = responder;
		}

		public HttpRequestMessage? LastRequest { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			LastRequest = request;
			var resp = _responder(request);
			return Task.FromResult(resp);
		}
	}
}