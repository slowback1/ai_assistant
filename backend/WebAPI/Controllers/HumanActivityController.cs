using Common.Interfaces;
using Common.Models;
using Logic.HumanActivity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("HumanActivity")]
public class HumanActivityController : ApplicationController
{
    private readonly ICrud<HumanActivity> _activityCrud;

    public HumanActivityController(ICrudFactory factory) : base(factory)
    {
        _activityCrud = Factory.GetCrud<HumanActivity>();
    }

    [HttpGet("")]
    public async Task<ActionResult> List()
    {
        var useCase = new ListHumanActivitiesUseCase(_activityCrud);
        var result = await useCase.Execute();
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
        var useCase = new GetHumanActivityUseCase(_activityCrud);
        var result = await useCase.Execute(id);
        return ToActionResult(result);
    }

    [HttpPost("")]
    public async Task<ActionResult> Create([FromBody] HumanActivity activity)
    {
        var useCase = new CreateHumanActivityUseCase(_activityCrud);
        var result = await useCase.Execute(activity);
        return ToActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] HumanActivity activity)
    {
        var useCase = new UpdateHumanActivityUseCase(_activityCrud);
        var result = await useCase.Execute(id, activity);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var useCase = new DeleteHumanActivityUseCase(_activityCrud);
        var result = await useCase.Execute(id);
        return ToActionResult(result);
    }
}
