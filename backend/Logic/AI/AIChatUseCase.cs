using System.Collections.Generic;
using Common.Interfaces;
using Common.Models;
using Logic.AI.PostProcessors;

namespace Logic.AI;

public class AIChatUseCase
{
	private readonly AIConfig _config;
	private readonly IAIRequester _requester;
	private readonly List<IAIPostProcessor> _postProcessors;

	public AIChatUseCase(AIConfig config, IAIRequester requester)
	{
		_config = config;
		_requester = requester;
		_postProcessors = [new RemoveThinkingPostProcessor()];
	}

	public UseCaseResult<AISession> Execute(AIRequest request, IPersonality personality)
	{
		var message = PersonalityPromptBuilder.BuildPrompt(personality, request.Message, request.Session);

		var context = new AIContext
		{
			Session = request.Session,
			Config = _config,
			Message = message
		};

		var result = _requester.GetAIResponse(context);

		context.AddSessionMessage(request.Message, "User");
		context.AddSessionMessage(result.Response, "Bot");
		foreach (var processor in _postProcessors)
		{
			result.Response = processor.PostProcess(result.Response);
		}

		return UseCaseResult<AISession>.Success(context.Session!);
	}
}