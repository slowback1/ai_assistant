using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public class MemoryExtractionService : IMemoryExtractionService
{
	private readonly AIConfig _config;
	private readonly IAIRequester _requester;
	private readonly MemoryConfig _memoryConfig;

	public MemoryExtractionService(AIConfig config, IAIRequester requester, MemoryConfig memoryConfig)
	{
		_config = config;
		_requester = requester;
		_memoryConfig = memoryConfig;
	}

	public Task<MemoryExtractionResponse> ExtractMemoriesAsync(MemoryExtractionRequest request)
	{
		var prompt = BuildMemoryExtractionPrompt(request);

		var context = new AIContext
		{
			Config = _config,
			Message = prompt
		};

		try
		{
			var result = _requester.GetAIResponse(context);

			if (string.IsNullOrEmpty(result.Response))
			{
				return Task.FromResult(new MemoryExtractionResponse());
			}

			// Try to parse JSON response
			var response = ParseMemoryExtractionResponse(result.Response);
			return Task.FromResult(response);
		}
		catch (Exception)
		{
			// If parsing fails, return empty response to not block story generation
			return Task.FromResult(new MemoryExtractionResponse());
		}
	}

	public Task<double> CalculateSimilarityAsync(string summary1, string summary2)
	{
		// Simple similarity calculation using AI prompt
		var prompt = $@"Rate the semantic similarity between these two statements on a scale of 0.0 to 1.0, where 0.0 means completely different and 1.0 means identical in meaning.

Statement 1: {summary1}
Statement 2: {summary2}

Respond with ONLY a number between 0.0 and 1.0, nothing else.";

		var context = new AIContext
		{
			Config = _config,
			Message = prompt
		};

		try
		{
			var result = _requester.GetAIResponse(context);
			
			if (double.TryParse(result.Response.Trim(), out var similarity))
			{
				return Task.FromResult(Math.Clamp(similarity, 0.0, 1.0));
			}
		}
		catch (Exception)
		{
			// On error, assume they're different
		}

		return Task.FromResult(0.0);
	}

	public async Task<IEnumerable<MemoryRecord>> FilterDuplicatesAsync(
		IEnumerable<CandidateMemory> candidates,
		IEnumerable<MemoryRecord> existingMemories,
		double similarityThreshold)
	{
		var filteredMemories = new List<MemoryRecord>();
		var existingMemoriesList = existingMemories.ToList();

		foreach (var candidate in candidates)
		{
			// Check confidence threshold
			if (candidate.Confidence < _memoryConfig.ConfidenceThreshold)
			{
				continue;
			}

			// Check for duplicates
			var isDuplicate = false;
			foreach (var existing in existingMemoriesList)
			{
				var similarity = await CalculateSimilarityAsync(candidate.Summary, existing.Summary);
				if (similarity >= similarityThreshold)
				{
					isDuplicate = true;
					break;
				}
			}

			if (!isDuplicate)
			{
				var memoryType = candidate.TypeSuggestion.Equals("LongTerm", StringComparison.OrdinalIgnoreCase)
					? MemoryType.LongTerm
					: MemoryType.ShortTerm;

				var memory = new MemoryRecord
				{
					Id = Guid.NewGuid().ToString(),
					Summary = candidate.Summary,
					Type = memoryType,
					Tags = candidate.Tags,
					ConfidenceScore = candidate.Confidence,
					CreatedAt = DateTime.UtcNow
				};

				filteredMemories.Add(memory);
			}
		}

		return filteredMemories;
	}

	private static string BuildMemoryExtractionPrompt(MemoryExtractionRequest request)
	{
		var sb = new StringBuilder();
		sb.AppendLine("You are a memory categorization system. Your task is to extract important facts, relationships, goals, and events from a story that should be remembered for future story continuity.");
		sb.AppendLine();
		sb.AppendLine("Personality Context:");
		sb.AppendLine(request.PersonalityProfile);
		sb.AppendLine();

		if (request.RecentEvents.Length > 0)
		{
			sb.AppendLine("Recent Events:");
			foreach (var evt in request.RecentEvents)
			{
				sb.AppendLine($"- {evt}");
			}
			sb.AppendLine();
		}

		sb.AppendLine("New Story:");
		sb.AppendLine(request.StoryOutput);
		sb.AppendLine();

		if (request.ExistingMemories.Length > 0)
		{
			sb.AppendLine("Existing Memories (avoid duplicates):");
			foreach (var memory in request.ExistingMemories)
			{
				sb.AppendLine($"- {memory}");
			}
			sb.AppendLine();
		}

		sb.AppendLine("Identify unique, important memories from the new story. Return a JSON object with this structure:");
		sb.AppendLine(@"{
  ""Candidates"": [
    {
      ""Summary"": ""brief summary of the fact/event/relationship"",
      ""TypeSuggestion"": ""ShortTerm or LongTerm"",
      ""Confidence"": 0.0-1.0,
      ""Tags"": [""tag1"", ""tag2""],
      ""Reasoning"": ""why this is important""
    }
  ]
}");
		sb.AppendLine();
		sb.AppendLine("Rules:");
		sb.AppendLine("- ShortTerm memories are temporary facts (current location, recent actions)");
		sb.AppendLine("- LongTerm memories are permanent facts (relationships, goals, achievements, character traits)");
		sb.AppendLine("- Confidence should be 0.7+ for memories worth keeping");
		sb.AppendLine("- Avoid duplicating existing memories");
		sb.AppendLine("- Focus on facts that affect future story continuity");
		sb.AppendLine();
		sb.AppendLine("Respond with ONLY valid JSON, no other text:");

		return sb.ToString();
	}

	private static MemoryExtractionResponse ParseMemoryExtractionResponse(string response)
	{
		// Try to extract JSON from the response (in case the AI adds extra text)
		var jsonStart = response.IndexOf('{');
		var jsonEnd = response.LastIndexOf('}');

		if (jsonStart >= 0 && jsonEnd > jsonStart)
		{
			var jsonText = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
			
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			return JsonSerializer.Deserialize<MemoryExtractionResponse>(jsonText, options) 
			       ?? new MemoryExtractionResponse();
		}

		return new MemoryExtractionResponse();
	}
}
