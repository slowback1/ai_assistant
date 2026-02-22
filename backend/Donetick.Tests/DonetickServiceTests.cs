using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Donetick.Tests;

public class DonetickServiceTests
{
    [Fact]
    public async Task GetOverdueChoresAsync_FiltersOverdueChores()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = @"[
                {
                    ""id"": 1,
                    ""name"": ""Overdue Chore"",
                    ""nextDueDate"": ""2026-01-01T00:00:00Z""
                },
                {
                    ""id"": 2,
                    ""name"": ""Future Chore"",
                    ""nextDueDate"": ""2027-01-01T00:00:00Z""
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
        var service = new DonetickService(donetickClient);

        var overdueChores = await service.GetOverdueChoresAsync();

        Assert.NotNull(overdueChores);
        Assert.Single(overdueChores);
        Assert.Equal(1, overdueChores[0].Id);
        Assert.Equal("Overdue Chore", overdueChores[0].Name);
    }

    [Fact]
    public async Task GetOverdueChoresAsync_ExcludesChoresWithoutDueDate()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = @"[
                {
                    ""id"": 1,
                    ""name"": ""Chore Without Due Date"",
                    ""nextDueDate"": null
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
        var service = new DonetickService(donetickClient);

        var overdueChores = await service.GetOverdueChoresAsync();

        Assert.NotNull(overdueChores);
        Assert.Empty(overdueChores);
    }

    [Fact]
    public async Task GetOverdueChoresAsync_OrdersByDueDate()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = @"[
                {
                    ""id"": 1,
                    ""name"": ""More Overdue"",
                    ""nextDueDate"": ""2026-01-01T00:00:00Z""
                },
                {
                    ""id"": 2,
                    ""name"": ""Less Overdue"",
                    ""nextDueDate"": ""2026-02-01T00:00:00Z""
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
        var service = new DonetickService(donetickClient);

        var overdueChores = await service.GetOverdueChoresAsync();

        Assert.NotNull(overdueChores);
        Assert.Equal(2, overdueChores.Count);
        Assert.Equal(1, overdueChores[0].Id);
        Assert.Equal(2, overdueChores[1].Id);
    }

    [Fact]
    public async Task CompleteChoreAsync_CallsClient()
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var client = new HttpClient(handler);
        var config = new DonetickConfig { ApiKey = "test-key", InstanceUrl = "https://test.local" };
        var donetickClient = new DonetickClient(config, client);
        var service = new DonetickService(donetickClient);

        var result = await service.CompleteChoreAsync(12);

        Assert.True(result);
    }

    [Fact]
    public void Constructor_ThrowsOnNullClient()
    {
        Assert.Throws<ArgumentNullException>(() => new DonetickService(null!));
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
