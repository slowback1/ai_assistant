using Common.Interfaces;
using Donetick;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("Donetick")]
public class DonetickController : ApplicationController
{
    private readonly DonetickService? _donetickService;

    public DonetickController(ICrudFactory crudFactory, DonetickService? donetickService = null) : base(crudFactory)
    {
        _donetickService = donetickService;
    }

    [HttpGet("OverdueChores")]
    public async Task<ActionResult> GetOverdueChores()
    {
        if (_donetickService == null)
        {
            return BadRequest(new { error = "Donetick integration is not configured" });
        }

        try
        {
            var chores = await _donetickService.GetOverdueChoresAsync();
            return Ok(chores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch overdue chores", details = ex.Message });
        }
    }

    [HttpPost("CompleteChore/{choreId}")]
    public async Task<ActionResult> CompleteChore(int choreId)
    {
        if (_donetickService == null)
        {
            return BadRequest(new { error = "Donetick integration is not configured" });
        }

        try
        {
            var success = await _donetickService.CompleteChoreAsync(choreId);
            
            if (success)
            {
                return Ok(new { success = true, message = "Chore completed successfully" });
            }
            
            return BadRequest(new { success = false, message = "Failed to complete chore" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to complete chore", details = ex.Message });
        }
    }
}
