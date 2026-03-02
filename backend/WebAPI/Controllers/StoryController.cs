using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using CharacterPersonalities;
using Logic.AI;
using AIChat;

namespace WebAPI.Controllers;

[Route("Story")]
public class StoryController : ApplicationController
{
	private readonly AIConfig _aiConfig;
	private readonly MemoryConfig _memoryConfig;
	private readonly IMemoryExtractionService? _memoryService;

	public StoryController(ICrudFactory crudFactory, AIConfig aiConfig, MemoryConfig memoryConfig, IMemoryExtractionService? memoryService = null) : base(crudFactory)
	{
		_aiConfig = aiConfig;
		_memoryConfig = memoryConfig;
		_memoryService = memoryService;
	}

	[HttpGet("Latest")]
	public async Task<ActionResult> GetLatestStory()
	{
		var storyCrud = Factory.GetCrud<StoryEvent>();
		var allEvents = await storyCrud.QueryAsync(_ => true);
		var latestEvent = allEvents.OrderByDescending(e => e.CreatedAt).FirstOrDefault();

		var madeUpEvent = new StoryEvent
		{
			Id = "latest",
			CreatedAt = DateTime.Now,
			Story = "This is some sample text to test things out.  Something about a quick brown fox and lazy dogs."
		};
		
		if (latestEvent == null)
		{
			latestEvent = madeUpEvent;
		}

		return Ok(latestEvent);
	}

	[HttpGet("All")]
	public async Task<ActionResult> GetAllStories()
	{
		var storyCrud = Factory.GetCrud<StoryEvent>();
		var allEvents = await storyCrud.QueryAsync(_ => true);
		var orderedEvents = allEvents.OrderByDescending(e => e.CreatedAt).ToList();

		return Ok(orderedEvents);
	}

	[HttpPost("Generate")]
	public async Task<ActionResult> GenerateStory([FromQuery] string? sessionId = null)
	{
		var storyCrud = Factory.GetCrud<StoryEvent>();
		var previousEvents = await storyCrud.QueryAsync(_ => true);
		var orderedEvents = previousEvents.OrderBy(e => e.CreatedAt).ToList();

		var personality = new DarthVader();
		// TODO: Implement proper personality ID system. Currently using name as ID.
		// Consider: 1) Adding Id property to IPersonality, or
		//          2) Using stable hash based on personality type name
		var personalityId = personality.Name;
		var requester = new AIChatRequester();
		
		StoryGenerationUseCase useCase;
		if (_memoryConfig.EnableMemoryExtraction && _memoryService != null)
		{
			useCase = new StoryGenerationUseCase(_aiConfig, requester, Factory, _memoryService, _memoryConfig);
		}
		else
		{
			useCase = new StoryGenerationUseCase(_aiConfig, requester);
		}

		var storyEventId = Guid.NewGuid().ToString();
		var result = await useCase.ExecuteAsync(personality, orderedEvents, personalityId, sessionId, storyEventId);

		if (result.Status == UseCaseStatus.Success)
		{
			var storyEvent = new StoryEvent
			{
				Id = storyEventId,
				Story = result.Result!,
				CreatedAt = DateTime.UtcNow
			};

			await storyCrud.CreateAsync(storyEvent);
			return Ok(storyEvent);
		}

		return BadRequest(new { error = result.ErrorMessage });
	}

	[HttpGet("Memories")]
	public async Task<ActionResult> GetMemories(
		[FromQuery] string? personalityId = null,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 50)
	{
		if (!_memoryConfig.EnableMemoryExtraction)
		{
			return BadRequest(new { error = "Memory system is not enabled" });
		}

		if (page < 1) page = 1;
		if (pageSize < 1 || pageSize > 100) pageSize = 50;

		var memoryCrud = Factory.GetCrud<MemoryRecord>();
		
		IEnumerable<MemoryRecord> memories;
		if (string.IsNullOrEmpty(personalityId))
		{
			memories = await memoryCrud.QueryAsync(_ => true);
		}
		else
		{
			memories = await memoryCrud.QueryAsync(m => m.PersonalityId == personalityId);
		}

		var orderedMemories = memories
			.OrderByDescending(m => m.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var totalCount = memories.Count();
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		return Ok(new
		{
			memories = orderedMemories,
			page,
			pageSize,
			totalCount,
			totalPages
		});
	}
}
