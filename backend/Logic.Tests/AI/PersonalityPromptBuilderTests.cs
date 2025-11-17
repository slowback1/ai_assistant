using Common.Models;
using Logic.AI;
using TestUtilities.TestData;

namespace Logic.Tests.AI;

public class PersonalityPromptBuilderTests
{
	[Test]
	public void BuildPrompt_WhenSessionIsNull_ProducesBasePrompt()
	{
		var personality = new TestPersonality("Zara", "witty conversationalist");
		var userMessage = "How are you?";

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, userMessage, null);

		StringAssert.StartsWith($"You are {personality.Name}, known as a {personality.Description}.\n", prompt);
		StringAssert.DoesNotContain("Previous conversation:", prompt);
		StringAssert.Contains($"User prompt: {userMessage}", prompt);
		StringAssert.EndsWith("Response: ", prompt);
	}

	[Test]
	public void BuildPrompt_WithSession_IncludesPreviousConversationAndMapsRoles()
	{
		var personality = new TestPersonality("Milo", "calm philosopher");
		var session = new AISession
		{
			SessionId = "s123",
			Messages = new[]
			{
				new AiMessage { Message = "Hi there", Role = "User" },
				new AiMessage { Message = "I ponder things", Role = "Assistant" }
			}
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Tell me a thought", session);

		StringAssert.Contains("Previous conversation:", prompt);
		// first message should remain labeled 'User'
		StringAssert.Contains("User: Hi there", prompt);
		// second message role != "User" should be labeled with personality name
		StringAssert.Contains($"{personality.Name}: I ponder things", prompt);
		StringAssert.Contains("User prompt: Tell me a thought", prompt);
	}

	[Test]
	public void BuildPrompt_WithEmptyMessages_StillProducesPrompt()
	{
		var personality = new TestPersonality("Ava", "direct");
		var session = new AISession { SessionId = "empty", Messages = new AiMessage[] { } };

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Ping", session);

		// Previous conversation header should be present but no messages after it
		StringAssert.Contains("Previous conversation:", prompt);
		StringAssert.Contains("User prompt: Ping", prompt);
	}
}