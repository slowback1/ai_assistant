using NUnit.Framework;
using Common.Interfaces;
using Common.Models;
using Logic.AI;
using TestUtilities.TestData;
using System.Linq;

namespace Logic.Tests.AI;

public class MemoryExtractionServiceTests
{
	[Test]
	public async Task ExtractMemoriesAsync_WithValidResponse_ParsesCorrectly()
	{
		var requester = new JsonAIRequester();
		var config = new AIConfig();
		var memoryConfig = new MemoryConfig { ConfidenceThreshold = 0.7 };
		var service = new MemoryExtractionService(config, requester, memoryConfig);

		var request = new MemoryExtractionRequest
		{
			PersonalityProfile = "Darth Vader",
			RecentEvents = new[] { "Entered the Death Star" },
			StoryOutput = "You confront Luke Skywalker in the throne room.",
			ExistingMemories = System.Array.Empty<string>()
		};

		var result = await service.ExtractMemoriesAsync(request);

		Assert.That(result.Candidates, Is.Not.Null);
		Assert.That(result.Candidates.Length, Is.GreaterThan(0));
		Assert.That(result.Candidates[0].Summary, Is.Not.Empty);
	}

	[Test]
	public async Task ExtractMemoriesAsync_WithEmptyResponse_ReturnsEmpty()
	{
		var requester = new EmptyAIRequester();
		var config = new AIConfig();
		var memoryConfig = new MemoryConfig();
		var service = new MemoryExtractionService(config, requester, memoryConfig);

		var request = new MemoryExtractionRequest
		{
			PersonalityProfile = "Test",
			RecentEvents = System.Array.Empty<string>(),
			StoryOutput = "Test story",
			ExistingMemories = System.Array.Empty<string>()
		};

		var result = await service.ExtractMemoriesAsync(request);

		Assert.That(result.Candidates, Is.Empty);
	}

	[Test]
	public async Task FilterDuplicatesAsync_FiltersLowConfidence()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var memoryConfig = new MemoryConfig { ConfidenceThreshold = 0.8 };
		var service = new MemoryExtractionService(config, requester, memoryConfig);

		var candidates = new[]
		{
			new CandidateMemory
			{
				Summary = "High confidence memory",
				TypeSuggestion = "LongTerm",
				Confidence = 0.9,
				Tags = new[] { "important" }
			},
			new CandidateMemory
			{
				Summary = "Low confidence memory",
				TypeSuggestion = "ShortTerm",
				Confidence = 0.5,
				Tags = new[] { "minor" }
			}
		};

		var result = await service.FilterDuplicatesAsync(
			candidates,
			System.Array.Empty<MemoryRecord>(),
			0.85);

		var memories = result.ToList();
		Assert.That(memories, Has.Count.EqualTo(1));
		Assert.That(memories[0].Summary, Is.EqualTo("High confidence memory"));
	}

	[Test]
	public async Task CalculateSimilarityAsync_ReturnsValidScore()
	{
		var requester = new SimilarityAIRequester();
		var config = new AIConfig();
		var memoryConfig = new MemoryConfig();
		var service = new MemoryExtractionService(config, requester, memoryConfig);

		var similarity = await service.CalculateSimilarityAsync("Test 1", "Test 2");

		Assert.That(similarity, Is.InRange(0.0, 1.0));
	}

	private class EmptyAIRequester : IAIRequester
	{
		public AIResult GetAIResponse(AIContext context)
		{
			return new AIResult { Response = string.Empty, SessionId = string.Empty };
		}
	}

	private class JsonAIRequester : IAIRequester
	{
		public AIResult GetAIResponse(AIContext context)
		{
			var jsonResponse = @"{
				""Candidates"": [
					{
						""Summary"": ""Confronted Luke in throne room"",
						""TypeSuggestion"": ""LongTerm"",
						""Confidence"": 0.95,
						""Tags"": [""event"", ""relationship""],
						""Reasoning"": ""Major confrontation""
					}
				]
			}";

			return new AIResult { Response = jsonResponse, SessionId = string.Empty };
		}
	}

	private class SimilarityAIRequester : IAIRequester
	{
		public AIResult GetAIResponse(AIContext context)
		{
			return new AIResult { Response = "0.75", SessionId = string.Empty };
		}
	}
}
