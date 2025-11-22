using NUnit.Framework;
using Common.Interfaces;
using Common.Models;
using Logic.AI;
using TestUtilities.TestData;

namespace Logic.Tests.AI;

public class StoryGenerationUseCaseTests
{
	[Test]
	public void Execute_WithNoEvents_GeneratesInitialStory()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new StoryGenerationUseCase(config, requester);
		var personality = new TestPersonality("Vader", "dark lord");

		var result = useCase.Execute(personality, new List<StoryEvent>());

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result, Is.Not.Null);
		Assert.That(result.Result, Does.StartWith("Echo: "));
		Assert.That(result.Result, Does.Contain("You are Vader, known as a dark lord."));
		Assert.That(result.Result, Does.Contain("beginning a new adventure"));
	}

	[Test]
	public void Execute_WithPreviousEvents_IncludesEventsInPrompt()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new StoryGenerationUseCase(config, requester);
		var personality = new TestPersonality("Hero", "brave warrior");

		var previousEvents = new List<StoryEvent>
		{
			new() { Id = "1", Story = "Defeated the dragon", CreatedAt = DateTime.UtcNow.AddHours(-2) },
			new() { Id = "2", Story = "Found the treasure", CreatedAt = DateTime.UtcNow.AddHours(-1) }
		};

		var result = useCase.Execute(personality, previousEvents);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result, Is.Not.Null);
		Assert.That(result.Result, Does.Contain("Previous events:"));
		Assert.That(result.Result, Does.Contain("Defeated the dragon"));
		Assert.That(result.Result, Does.Contain("Found the treasure"));
		Assert.That(result.Result, Does.Contain("what you do next"));
	}

	[Test]
	public void Execute_IncludesPersonalityInPrompt()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var useCase = new StoryGenerationUseCase(config, requester);
		var personality = new TestPersonality("Wizard", "powerful spellcaster");

		var result = useCase.Execute(personality, new List<StoryEvent>());

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result, Does.Contain("You are Wizard, known as a powerful spellcaster."));
	}

	[Test]
	public void Execute_WithEmptyResponse_ReturnsFailure()
	{
		var requester = new EmptyAIRequester();
		var config = new AIConfig();
		var useCase = new StoryGenerationUseCase(config, requester);
		var personality = new TestPersonality("Test", "test");

		var result = useCase.Execute(personality, new List<StoryEvent>());

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Failed to generate story"));
	}

	private class EmptyAIRequester : IAIRequester
	{
		public AIResult GetAIResponse(AIContext context)
		{
			return new AIResult { Response = string.Empty, SessionId = string.Empty };
		}
	}
}
