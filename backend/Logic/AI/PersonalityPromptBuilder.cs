using System.Text;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public static class PersonalityPromptBuilder
{
	public static string BuildPrompt(IPersonality personality, string userMessage, AISession? session)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append($"You are {personality.Name}, known as a {personality.Description}.\n");

		if (session != null)
		{
			stringBuilder.Append("Previous conversation:\n");
			foreach (var msg in session.Messages)
			{
				var role = msg.Role == "User" ? "User" : personality.Name;
				stringBuilder.Append($"{role}: {msg.Message}\n");
			}
		}

		stringBuilder.Append($"User prompt: {userMessage}");
		stringBuilder.Append("Response: ");

		return stringBuilder.ToString();
	}
}