using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Donetick.Tests;

public class DonetickClientTests
{
    [Fact]
    public async Task GetAllChoresAsync_ReturnsChores()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = @"[
                {
                    ""id"": 1,
                    ""name"": ""Test Chore"",
                    ""nextDueDate"": ""2026-02-14T00:00:00Z""
                }
            ]";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "test-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);

        var chores = await donetickClient.GetAllChoresAsync();

        Assert.NotNull(chores);
        Assert.Single(chores);
        Assert.Equal(1, chores[0].Id);
        Assert.Equal("Test Chore", chores[0].Name);
    }

    [Fact]
    public async Task GetAllChoresAsync_AddsSecretKeyHeader()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "my-secret-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);

        await donetickClient.GetAllChoresAsync();

        Assert.NotNull(handler.LastRequest);
        Assert.True(handler.LastRequest!.Headers.Contains("secretkey"));
        var headerValue = handler.LastRequest.Headers.GetValues("secretkey");
        Assert.Contains("my-secret-key", headerValue);
    }

    [Fact]
    public async Task CompleteChoreAsync_ReturnsTrue_OnSuccess()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "test-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);

        var result = await donetickClient.CompleteChoreAsync(12);

        Assert.True(result);
    }

    [Fact]
    public async Task CompleteChoreAsync_ReturnsFalse_OnFailure()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "test-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);

        var result = await donetickClient.CompleteChoreAsync(12);

        Assert.False(result);
    }

    [Fact]
    public async Task CompleteChoreAsync_AddsSecretKeyHeader()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "my-secret-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);

        await donetickClient.CompleteChoreAsync(12);

        Assert.NotNull(handler.LastRequest);
        Assert.True(handler.LastRequest!.Headers.Contains("secretkey"));
        var headerValue = handler.LastRequest.Headers.GetValues("secretkey");
        Assert.Contains("my-secret-key", headerValue);
    }

    [Fact]
    public void Constructor_ThrowsOnNullConfig()
    {
        Assert.Throws<ArgumentNullException>(() => new DonetickClient(null!));
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
