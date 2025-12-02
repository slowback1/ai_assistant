using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public class StoryGenerationUseCase
{
	private readonly AIConfig _config;
	private readonly IAIRequester _requester;

	public StoryGenerationUseCase(AIConfig config, IAIRequester requester)
	{
		_config = config;
		_requester = requester;
	}

	public UseCaseResult<string> Execute(IPersonality personality, IEnumerable<StoryEvent> previousEvents)
	{
		var prompt = BuildStoryPrompt(personality, previousEvents);

		var context = new AIContext
		{
			Config = _config,
			Message = prompt
		};

		var result = _requester.GetAIResponse(context);

		if (string.IsNullOrEmpty(result.Response))
		{
			return UseCaseResult<string>.Failure("Failed to generate story");
		}

		return UseCaseResult<string>.Success(result.Response);
	}

	private static string BuildStoryPrompt(IPersonality personality, IEnumerable<StoryEvent> previousEvents)
	{
		var stringBuilder = PersonalityPromptBuilder.BuildPersonalityPrompt(personality); 

		var eventsList = previousEvents.ToList();
		
		if (!eventsList.Any())
		{
			stringBuilder.Append("You are beginning a new adventure. Describe in 2-3 sentences what you are doing right now and what happens next.\n");
		}
		else
		{
			stringBuilder.Append("Previous events:\n");
			foreach (var evt in eventsList)
			{
				stringBuilder.Append($"- {evt.Story}\n");
			}
			stringBuilder.Append("\nBased on these events, describe in 2-3 sentences what you do next.\n");
		}

		stringBuilder.Append("Response: ");

		return stringBuilder.ToString();
	}
}
