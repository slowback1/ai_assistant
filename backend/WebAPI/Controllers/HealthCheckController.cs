using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class HealthCheckResult
{
    public string Status { get; init; } = string.Empty;
    public DateTime ResponseDate { get; set; } = DateTime.Now;
}

[Route("HealthCheck")]
public class HealthCheckController : ApplicationController
{
    private readonly AIConfig _aiConfig;

    public HealthCheckController(ICrudFactory factory, AIConfig aiConfig) : base(factory)
    {
        _aiConfig = aiConfig;
    }

    [HttpGet]
    [Route("")]
    public ActionResult HealthCheck()
    {
        return Ok(new HealthCheckResult { Status = "Healthy" });
    }

    [HttpGet]
    [Route("AIConfig")]
    public ActionResult GetAIConfig()
    {
        return Ok(_aiConfig);
    }
}