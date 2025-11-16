using AIChat;
using CharacterPersonalities;
using Common.Interfaces;
using Common.Models;
using Logic.AI;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class SimpleAIRequest
{
	public string Prompt { get; set; } = string.Empty;
}

[Microsoft.AspNetCore.Components.Route("AI")]
public class AIController : ApplicationController
{
	private readonly AIConfig _aiConfig;

	public AIController(ICrudFactory crudFactory, AIConfig aiConfig) : base(crudFactory)
	{
		_aiConfig = aiConfig;
	}

	[HttpPost("Simple")]
	public ActionResult SimpleRequest(SimpleAIRequest request)
	{
		var requester = new AIChatRequester();
		var context = new AIContext
		{
			Config = _aiConfig,
			Message = request.Prompt
		};
		var response = requester.GetAIResponse(context);
		return Ok(response);
	}

	[HttpPost("ChatWithDarthVader")]
	public ActionResult ChatWithDarthVader([FromBody] AIRequest request)
	{
		var useCase = new AIChatUseCase(_aiConfig, new AIChatRequester());
		var personality = new DarthVader();
		var result = useCase.Execute(request, personality);
		return ToActionResult(result);
	}
}