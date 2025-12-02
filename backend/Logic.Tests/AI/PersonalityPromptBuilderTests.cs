using System;
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

	[Test]
	public void BuildPrompt_WithAge_IncludesAgeInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Age = "30 years old"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Age: 30 years old", prompt);
	}

	[Test]
	public void BuildPrompt_WithPhysicalFeatures_IncludesPhysicalFeaturesInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			PhysicalFeatures = "Tall with brown hair"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Physical features: Tall with brown hair", prompt);
	}

	[Test]
	public void BuildPrompt_WithLoves_IncludesLovesInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Loves = new[] { "Music", "Art" }
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Loves: Music, Art", prompt);
	}

	[Test]
	public void BuildPrompt_WithLikes_IncludesLikesInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Likes = new[] { "Coffee", "Reading" }
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Likes: Coffee, Reading", prompt);
	}

	[Test]
	public void BuildPrompt_WithDislikes_IncludesDislikesInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Dislikes = new[] { "Noise", "Crowds" }
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Dislikes: Noise, Crowds", prompt);
	}

	[Test]
	public void BuildPrompt_WithHates_IncludesHatesInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Hates = new[] { "Dishonesty", "Laziness" }
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Hates: Dishonesty, Laziness", prompt);
	}

	[Test]
	public void BuildPrompt_WithRelationships_IncludesRelationshipsInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Relationships = new[] { "Father: John", "Friend: Jane" }
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Relationships: Father: John, Friend: Jane", prompt);
	}

	[Test]
	public void BuildPrompt_WithTypicalClothing_IncludesTypicalClothingInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			TypicalClothing = "Casual jeans and t-shirt"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Typical clothing: Casual jeans and t-shirt", prompt);
	}

	[Test]
	public void BuildPrompt_WithDreamsForFuture_IncludesDreamsForFutureInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			DreamsForFuture = "To travel the world"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Dreams for the future: To travel the world", prompt);
	}

	[Test]
	public void BuildPrompt_WithWhatTheyWillTalkAbout_IncludesWhatTheyWillTalkAboutInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			WhatTheyWillTalkAbout = "Technology and science"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("What they will talk about: Technology and science", prompt);
	}

	[Test]
	public void BuildPrompt_WithOccupation_IncludesOccupationInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Occupation = "Software Engineer"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Occupation: Software Engineer", prompt);
	}

	[Test]
	public void BuildPrompt_WithBackground_IncludesBackgroundInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Background = "Grew up in a small town"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Background: Grew up in a small town", prompt);
	}

	[Test]
	public void BuildPrompt_WithMotivations_IncludesMotivationsInPrompt()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Motivations = "To help others succeed"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.Contains("Motivations: To help others succeed", prompt);
	}

	[Test]
	public void BuildPrompt_SkipsNullTraits()
	{
		var personality = new TestPersonality("Test", "test description");
		// All new traits are null by default

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.DoesNotContain("Age:", prompt);
		StringAssert.DoesNotContain("Physical features:", prompt);
		StringAssert.DoesNotContain("Loves:", prompt);
		StringAssert.DoesNotContain("Likes:", prompt);
		StringAssert.DoesNotContain("Dislikes:", prompt);
		StringAssert.DoesNotContain("Hates:", prompt);
		StringAssert.DoesNotContain("Relationships:", prompt);
		StringAssert.DoesNotContain("Typical clothing:", prompt);
		StringAssert.DoesNotContain("Dreams for the future:", prompt);
		StringAssert.DoesNotContain("What they will talk about:", prompt);
		StringAssert.DoesNotContain("Occupation:", prompt);
		StringAssert.DoesNotContain("Background:", prompt);
		StringAssert.DoesNotContain("Motivations:", prompt);
	}

	[Test]
	public void BuildPrompt_SkipsEmptyStringTraits()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Age = "",
			PhysicalFeatures = "   ",
			Occupation = ""
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.DoesNotContain("Age:", prompt);
		StringAssert.DoesNotContain("Physical features:", prompt);
		StringAssert.DoesNotContain("Occupation:", prompt);
	}

	[Test]
	public void BuildPrompt_SkipsEmptyListTraits()
	{
		var personality = new TestPersonality("Test", "test description")
		{
			Loves = Array.Empty<string>(),
			Likes = Array.Empty<string>()
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		StringAssert.DoesNotContain("Loves:", prompt);
		StringAssert.DoesNotContain("Likes:", prompt);
	}

	[Test]
	public void BuildPrompt_WithAllTraits_IncludesAllTraitsInCorrectOrder()
	{
		var personality = new TestPersonality("CompleteCharacter", "fully fleshed out")
		{
			Age = "25",
			PhysicalFeatures = "Average build",
			Loves = new[] { "Family" },
			Likes = new[] { "Books" },
			Dislikes = new[] { "Rain" },
			Hates = new[] { "Lies" },
			Relationships = new[] { "Partner: Alex" },
			TypicalClothing = "Business casual",
			DreamsForFuture = "World peace",
			WhatTheyWillTalkAbout = "Philosophy",
			Occupation = "Teacher",
			Background = "Urban upbringing",
			Motivations = "Knowledge"
		};

		var prompt = PersonalityPromptBuilder.BuildPrompt(personality, "Hello", null);

		// Check that all traits are present
		StringAssert.Contains("Age: 25", prompt);
		StringAssert.Contains("Physical features: Average build", prompt);
		StringAssert.Contains("Loves: Family", prompt);
		StringAssert.Contains("Likes: Books", prompt);
		StringAssert.Contains("Dislikes: Rain", prompt);
		StringAssert.Contains("Hates: Lies", prompt);
		StringAssert.Contains("Relationships: Partner: Alex", prompt);
		StringAssert.Contains("Typical clothing: Business casual", prompt);
		StringAssert.Contains("Dreams for the future: World peace", prompt);
		StringAssert.Contains("What they will talk about: Philosophy", prompt);
		StringAssert.Contains("Occupation: Teacher", prompt);
		StringAssert.Contains("Background: Urban upbringing", prompt);
		StringAssert.Contains("Motivations: Knowledge", prompt);

		// Check order: Age should come before Physical features, etc.
		var ageIndex = prompt.IndexOf("Age:");
		var physicalIndex = prompt.IndexOf("Physical features:");
		var lovesIndex = prompt.IndexOf("Loves:");
		var likesIndex = prompt.IndexOf("Likes:");
		var occupationIndex = prompt.IndexOf("Occupation:");
		var motivationsIndex = prompt.IndexOf("Motivations:");
		var userPromptIndex = prompt.IndexOf("User prompt:");

		Assert.That(ageIndex, Is.LessThan(physicalIndex));
		Assert.That(physicalIndex, Is.LessThan(lovesIndex));
		Assert.That(lovesIndex, Is.LessThan(likesIndex));
		Assert.That(occupationIndex, Is.LessThan(motivationsIndex));
		Assert.That(motivationsIndex, Is.LessThan(userPromptIndex));
	}
}