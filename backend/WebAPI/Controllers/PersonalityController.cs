using Common.Interfaces;
using Common.Models;
using Logic.Personality;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("Personality")]
public class PersonalityController : ApplicationController
{
	private readonly ICrud<Personality> _personalityCrud;

	public PersonalityController(ICrudFactory factory) : base(factory)
	{
		_personalityCrud = Factory.GetCrud<Personality>();
	}

	[HttpPost("")]
	public async Task<ActionResult> Create([FromBody] Personality personality)
	{
		var useCase = new CreatePersonalityUseCase(_personalityCrud);
		var result = await useCase.Execute(personality);
		return ToActionResult(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> GetById(string id)
	{
		var useCase = new GetPersonalityByIdUseCase(_personalityCrud);
		var result = await useCase.Execute(id);
		return ToActionResult(result);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(string id, [FromBody] Personality personality)
	{
		var useCase = new UpdatePersonalityUseCase(_personalityCrud);
		var result = await useCase.Execute(id, personality);
		return ToActionResult(result);
	}

	[HttpGet("paginate")]
	public async Task<ActionResult> Paginate(
		[FromQuery] int page = 1, 
		[FromQuery] int pageSize = 10, 
		[FromQuery] string? nameFilter = null,
		[FromQuery] string? sortBy = "name",
		[FromQuery] string? sortOrder = "asc")
	{
		var useCase = new PaginatePersonalitiesUseCase(_personalityCrud);
		var result = await useCase.Execute(page, pageSize, nameFilter, sortBy, sortOrder);
		return ToActionResult(result);
	}

	[HttpPost("{id}/setactive")]
	public async Task<ActionResult> SetActive(string id)
	{
		var allPersonalities = await _personalityCrud.QueryAsync(_ => true);
		
		foreach (var personality in allPersonalities)
		{
			personality.IsActive = personality.Id == id;
			await _personalityCrud.UpdateAsync(personality.Id, personality);
		}
		
		var updatedPersonality = allPersonalities.FirstOrDefault(p => p.Id == id);
		if (updatedPersonality == null)
		{
			return NotFound(new { error = "Personality not found" });
		}
		
		return Ok(updatedPersonality);
	}
}
