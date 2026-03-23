using Common.Interfaces;
using Common.Models;
using Logic.Activity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("Activity")]
public class ActivityController : ApplicationController
{
	private readonly ICrud<Common.Models.Activity> _activityCrud;
	private readonly ICrud<ActivityPickerState> _stateCrud;

	public ActivityController(ICrudFactory factory) : base(factory)
	{
		_activityCrud = Factory.GetCrud<Common.Models.Activity>();
		_stateCrud = Factory.GetCrud<ActivityPickerState>();
	}

	[HttpGet("")]
	public async Task<ActionResult> GetAll()
	{
		var useCase = new GetAllActivitiesUseCase(_activityCrud);
		var result = await useCase.Execute();
		return ToActionResult(result);
	}

	[HttpPost("")]
	public async Task<ActionResult> Create([FromBody] CreateActivityRequest request)
	{
		var useCase = new CreateActivityUseCase(_activityCrud, _stateCrud);
		var result = await useCase.Execute(request.Name);
		return ToActionResult(result);
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(string id)
	{
		var useCase = new DeleteActivityUseCase(_activityCrud, _stateCrud);
		var result = await useCase.Execute(id);
		return ToActionResult(result);
	}

	[HttpGet("Current")]
	public async Task<ActionResult> GetCurrent()
	{
		var useCase = new GetCurrentActivityUseCase(_activityCrud, _stateCrud);
		var result = await useCase.Execute();
		return ToActionResult(result);
	}

	[HttpPost("PickNow")]
	public async Task<ActionResult> PickNow()
	{
		var useCase = new PickNextActivityUseCase(_activityCrud, _stateCrud);
		var result = await useCase.Execute();
		return ToActionResult(result);
	}
}

public record CreateActivityRequest(string Name);
