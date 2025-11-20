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

	public StoryController(ICrudFactory crudFactory, AIConfig aiConfig) : base(crudFactory)
	{
		_aiConfig = aiConfig;
	}

	[HttpGet("Latest")]
	public async Task<ActionResult> GetLatestStory()
	{
		var storyCrud = Factory.GetCrud<StoryEvent>();
		var allEvents = await storyCrud.QueryAsync(_ => true);
		var latestEvent = allEvents.OrderByDescending(e => e.CreatedAt).FirstOrDefault();

		if (latestEvent == null)
		{
			return NotFound(new { message = "No stories found" });
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
	public async Task<ActionResult> GenerateStory()
	{
		var storyCrud = Factory.GetCrud<StoryEvent>();
		var previousEvents = await storyCrud.QueryAsync(_ => true);
		var orderedEvents = previousEvents.OrderBy(e => e.CreatedAt).ToList();

		var personality = new DarthVader();
		var requester = new AIChatRequester();
		var useCase = new StoryGenerationUseCase(_aiConfig, requester);

		var result = useCase.Execute(personality, orderedEvents);

		if (result.Status == UseCaseStatus.Success)
		{
			var storyEvent = new StoryEvent
			{
				Id = Guid.NewGuid().ToString(),
				Story = result.Result!,
				CreatedAt = DateTime.UtcNow
			};

			await storyCrud.CreateAsync(storyEvent);
			return Ok(storyEvent);
		}

		return BadRequest(new { error = result.ErrorMessage });
	}
}
