using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Microsoft.AspNetCore.Components.Route("Story")]
public class StoryController : ApplicationController
{
	public StoryController(ICrudFactory crudFactory) : base(crudFactory)
	{
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
}
