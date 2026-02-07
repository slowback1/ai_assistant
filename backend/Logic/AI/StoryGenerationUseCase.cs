using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public class StoryGenerationUseCase
{
	private readonly AIConfig _config;
	private readonly IAIRequester _requester;
	private readonly ICrudFactory? _crudFactory;
	private readonly IMemoryExtractionService? _memoryService;
	private readonly MemoryConfig? _memoryConfig;

	public StoryGenerationUseCase(AIConfig config, IAIRequester requester)
	{
		_config = config;
		_requester = requester;
	}

	public StoryGenerationUseCase(
		AIConfig config,
		IAIRequester requester,
		ICrudFactory crudFactory,
		IMemoryExtractionService memoryService,
		MemoryConfig memoryConfig)
	{
		_config = config;
		_requester = requester;
		_crudFactory = crudFactory;
		_memoryService = memoryService;
		_memoryConfig = memoryConfig;
	}

	public UseCaseResult<string> Execute(IPersonality personality, IEnumerable<StoryEvent> previousEvents)
	{
		return ExecuteAsync(personality, previousEvents, null, null, null).GetAwaiter().GetResult();
	}

	public async Task<UseCaseResult<string>> ExecuteAsync(
		IPersonality personality,
		IEnumerable<StoryEvent> previousEvents,
		string? personalityId,
		string? sessionId,
		string? storyEventId)
	{
		var eventsList = previousEvents.ToList();
		
		// Load long-term memories if enabled
		var longTermMemories = new List<MemoryRecord>();
		if (_memoryConfig?.EnableMemoryExtraction == true && _crudFactory != null && !string.IsNullOrEmpty(personalityId))
		{
			try
			{
				var memoryCrud = _crudFactory.GetCrud<MemoryRecord>();
				var allMemories = await memoryCrud.QueryAsync(m =>
					m.PersonalityId == personalityId &&
					m.Type == MemoryType.LongTerm);
				
				longTermMemories = allMemories
					.OrderByDescending(m => m.CreatedAt)
					.Take(_memoryConfig.MaxMemoriesPerPrompt)
					.ToList();
			}
			catch (Exception)
			{
				// Continue without memories if retrieval fails
			}
		}

		var prompt = BuildStoryPrompt(personality, eventsList, longTermMemories);

		var context = new AIContext
		{
			Config = _config,
			Message = prompt
		};

		var result = _requester.GetAIResponse(context);

		if (string.IsNullOrEmpty(result.Response))
		{
			return UseCaseResult<string>.Failure("Failed to generate story");
		}

		// Extract memories if enabled
		if (_memoryConfig?.EnableMemoryExtraction == true &&
		    _memoryService != null &&
		    _crudFactory != null &&
		    !string.IsNullOrEmpty(personalityId) &&
		    !string.IsNullOrEmpty(storyEventId))
		{
			_ = Task.Run(async () => await ExtractAndStoreMemoriesAsync(
				personality,
				eventsList,
				result.Response,
				personalityId,
				sessionId,
				storyEventId));
		}

		return UseCaseResult<string>.Success(result.Response);
	}

	private async Task ExtractAndStoreMemoriesAsync(
		IPersonality personality,
		List<StoryEvent> previousEvents,
		string storyOutput,
		string personalityId,
		string? sessionId,
		string storyEventId)
	{
		if (_memoryService == null || _crudFactory == null || _memoryConfig == null)
		{
			return;
		}

		try
		{
			var memoryCrud = _crudFactory.GetCrud<MemoryRecord>();
			
			// Get existing memories for deduplication
			var existingMemories = await memoryCrud.QueryAsync(m => m.PersonalityId == personalityId);
			var existingMemoriesList = existingMemories.ToList();

			// Build personality profile for extraction
			var personalityProfile = PersonalityPromptBuilder.BuildPersonalityPrompt(personality).ToString();

			// Get recent events for context
			var recentEvents = previousEvents
				.OrderByDescending(e => e.CreatedAt)
				.Take(_memoryConfig.ShortTermMemoryCount)
				.Select(e => e.Story)
				.ToArray();

			var extractionRequest = new MemoryExtractionRequest
			{
				PersonalityProfile = personalityProfile,
				RecentEvents = recentEvents,
				StoryOutput = storyOutput,
				ExistingMemories = existingMemoriesList.Select(m => m.Summary).ToArray()
			};

			var extractionResponse = await _memoryService.ExtractMemoriesAsync(extractionRequest);

			// Filter and store non-duplicate memories
			var newMemories = await _memoryService.FilterDuplicatesAsync(
				extractionResponse.Candidates,
				existingMemoriesList,
				_memoryConfig.SimilarityThreshold);

			foreach (var memory in newMemories)
			{
				memory.PersonalityId = personalityId;
				memory.SessionId = sessionId;
				memory.SourceStoryEventId = storyEventId;
				
				await memoryCrud.CreateAsync(memory);
			}
		}
		catch (Exception)
		{
			// Silently fail memory extraction to not block story generation
		}
	}

	private static string BuildStoryPrompt(IPersonality personality, List<StoryEvent> previousEvents, List<MemoryRecord> longTermMemories)
	{
		var stringBuilder = PersonalityPromptBuilder.BuildPersonalityPrompt(personality);

		// Add long-term memories section
		if (longTermMemories.Any())
		{
			stringBuilder.Append("\nLong-term memories:\n");
			foreach (var memory in longTermMemories)
			{
				stringBuilder.Append($"- {memory.Summary}\n");
			}
			stringBuilder.Append("\n");
		}

		if (!previousEvents.Any())
		{
			stringBuilder.Append("You are beginning a new adventure. Describe in 2-3 sentences what you are doing right now and what happens next.\n");
		}
		else
		{
			stringBuilder.Append("Previous events:\n");
			foreach (var evt in previousEvents)
			{
				stringBuilder.Append($"- {evt.Story}\n");
			}
			stringBuilder.Append("\nBased on these events, describe in 2-3 sentences what you do next.\n");
		}

		stringBuilder.Append("Response: ");

		return stringBuilder.ToString();
	}
}
