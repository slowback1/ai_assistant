using System.Text;
using Common.Interfaces;
using Common.Models;

namespace Logic.AI;

public static class PersonalityPromptBuilder
{
	public static string BuildPrompt(IPersonality personality, string userMessage, AISession? session)
	{
		var stringBuilder = BuildPersonalityPrompt(personality);

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

	public static StringBuilder BuildPersonalityPrompt(IPersonality personality)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append($"You are {personality.Name}, known as a {personality.Description}.\n");

		AppendIfNotEmpty(stringBuilder, "Age", personality.Age);
		AppendIfNotEmpty(stringBuilder, "Physical features", personality.PhysicalFeatures);
		AppendListIfNotEmpty(stringBuilder, "Loves", personality.Loves);
		AppendListIfNotEmpty(stringBuilder, "Likes", personality.Likes);
		AppendListIfNotEmpty(stringBuilder, "Dislikes", personality.Dislikes);
		AppendListIfNotEmpty(stringBuilder, "Hates", personality.Hates);
		AppendListIfNotEmpty(stringBuilder, "Relationships", personality.Relationships);
		AppendIfNotEmpty(stringBuilder, "Typical clothing", personality.TypicalClothing);
		AppendIfNotEmpty(stringBuilder, "Dreams for the future", personality.DreamsForFuture);
		AppendIfNotEmpty(stringBuilder, "What they will talk about", personality.WhatTheyWillTalkAbout);
		AppendIfNotEmpty(stringBuilder, "Occupation", personality.Occupation);
		AppendIfNotEmpty(stringBuilder, "Background", personality.Background);
		AppendIfNotEmpty(stringBuilder, "Motivations", personality.Motivations);
		return stringBuilder;
	}

	private static void AppendIfNotEmpty(StringBuilder sb, string label, string? value)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			sb.Append($"{label}: {value}\n");
		}
	}

	private static void AppendListIfNotEmpty(StringBuilder sb, string label, System.Collections.Generic.IReadOnlyList<string>? values)
	{
		if (values != null && values.Count > 0)
		{
			sb.Append($"{label}: {string.Join(", ", values)}\n");
		}
	}
}