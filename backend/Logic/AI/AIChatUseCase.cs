using System.Text;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public class AIChatUseCase
{
	private readonly AIConfig _config;
	private readonly IAIRequester _requester;

	public AIChatUseCase(AIConfig config, IAIRequester requester)
	{
		_config = config;
		_requester = requester;
	}

	public UseCaseResult<AISession> Execute(AIRequest request, IPersonality personality)
	{
		var message = PersonalityPromptBuilder.BuildPrompt(personality, request.Message);

		var context = new AIContext
		{
			Session = request.Session,
			Config = _config,
			Message = message
		};

		var result = _requester.GetAIResponse(context);

		context.AddSessionMessage(request.Message, "User");
		context.AddSessionMessage(result.Response, "Bot");

		return UseCaseResult<AISession>.Success(context.Session!);
	}
}

internal static class PersonalityPromptBuilder
{
	internal static string BuildPrompt(IPersonality personality, string userMessage)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append($"You are {personality.Name}, known as a {personality.Description}.\n");
		stringBuilder.Append($"User prompt: {userMessage}");
		stringBuilder.Append("Response: ");

		return stringBuilder.ToString();
	}
}