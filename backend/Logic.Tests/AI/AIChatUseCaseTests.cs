using NUnit.Framework;
using Common.Models;
using TestUtilities.TestData;

namespace Logic.Tests.AI;

public class AIChatUseCaseTests
{
	[Test]
	public void Execute_WhenNoSession_CreatesSessionAndAddsMessages()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new Logic.AI.AIChatUseCase(config, requester);
		var personality = new TestPersonality("Zed", "friendly helper");

		var request = new AIRequest { Message = "Hello world" };
		var result = useCase.Execute(request, personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		var session = result.Result!;
		Assert.That(session, Is.Not.Null);
		Assert.That(session.SessionId, Is.Not.Null.And.Not.Empty);
		Assert.That(session.Messages.Length, Is.EqualTo(2));

		// first message is the original user message
		Assert.That(session.Messages[0].Message, Is.EqualTo("Hello world"));
		Assert.That(session.Messages[0].Role, Is.EqualTo("User"));

		// second message is the bot response from TestAIRequester which echoes the prompt
		Assert.That(session.Messages[1].Message, Does.StartWith("Echo: "));
		Assert.That(session.Messages[1].Role, Is.EqualTo("Bot"));
		// ensure the prompt that was echoed contains the user's original prompt fragment
		Assert.That(session.Messages[1].Message, Does.Contain("User prompt: Hello world"));
	}

	[Test]
	public void Execute_WithExistingSession_AppendsMessagesPreservingPrevious()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new Logic.AI.AIChatUseCase(config, requester);
		var personality = new TestPersonality("Luna", "curious guide");

		var existing = new AISession
		{
			SessionId = "session-123",
			Messages = new[] { new AiMessage { Message = "previous", Role = "User" } }
		};

		var request = new AIRequest { Session = existing, Message = "Follow up" };
		var result = useCase.Execute(request, personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		var session = result.Result!;
		Assert.That(session.SessionId, Is.EqualTo("session-123"));
		// original message should remain first, then new user and bot messages appended
		Assert.That(session.Messages.Length, Is.EqualTo(3));
		Assert.That(session.Messages[0].Message, Is.EqualTo("previous"));
		Assert.That(session.Messages[1].Message, Is.EqualTo("Follow up"));
		Assert.That(session.Messages[1].Role, Is.EqualTo("User"));
		Assert.That(session.Messages[2].Role, Is.EqualTo("Bot"));
		Assert.That(session.Messages[2].Message, Does.Contain("User prompt: Follow up"));
	}

	[Test]
	public void Execute_IncludesPersonalityInfoInBuiltPrompt()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new Logic.AI.AIChatUseCase(config, requester);
		var personality = new TestPersonality("EchoBot", "playful mimic");

		var request = new AIRequest { Message = "Tell me something" };
		var result = useCase.Execute(request, personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		var session = result.Result!;
		// the TestAIRequester echoes the entire built prompt, so the bot message should contain the personality intro line
		Assert.That(session.Messages[1].Message, Does.Contain($"You are {personality.Name}, known as a {personality.Description}."));
	}
}