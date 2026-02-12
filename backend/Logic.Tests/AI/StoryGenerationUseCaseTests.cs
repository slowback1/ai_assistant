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

	[Test]
	public async Task ExecuteAsync_WithMemoriesEnabled_IncludesMemoriesInPrompt()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var crudFactory = new TestCrudFactory();
		var memoryService = new TestMemoryExtractionService();
		var memoryConfig = new MemoryConfig { EnableMemoryExtraction = true, MaxMemoriesPerPrompt = 5 };
		
		// Pre-populate a memory
		var memoryCrud = crudFactory.GetCrud<MemoryRecord>();
		await memoryCrud.CreateAsync(new MemoryRecord
		{
			Id = "mem-1",
			Summary = "Previously defeated a dragon",
			Type = MemoryType.LongTerm,
			PersonalityId = "vader-1",
			CreatedAt = DateTime.UtcNow.AddDays(-1)
		});

		var useCase = new StoryGenerationUseCase(config, requester, crudFactory, memoryService, memoryConfig);
		var personality = new TestPersonality("Vader", "dark lord");

		var result = await useCase.ExecuteAsync(personality, new List<StoryEvent>(), "vader-1", "session-1", "event-1");

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result, Is.Not.Null);
		Assert.That(result.Result, Does.Contain("Long-term memories:"));
		Assert.That(result.Result, Does.Contain("Previously defeated a dragon"));
	}

	[Test]
	public async Task ExecuteAsync_WithMemoriesDisabled_DoesNotIncludeMemories()
	{
		var requester = new TestAIRequester();
		var config = new AIConfig();
		var crudFactory = new TestCrudFactory();
		var memoryService = new TestMemoryExtractionService();
		var memoryConfig = new MemoryConfig { EnableMemoryExtraction = false };

		var useCase = new StoryGenerationUseCase(config, requester, crudFactory, memoryService, memoryConfig);
		var personality = new TestPersonality("Vader", "dark lord");

		var result = await useCase.ExecuteAsync(personality, new List<StoryEvent>(), "vader-1", "session-1", "event-1");

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result, Is.Not.Null);
		Assert.That(result.Result, Does.Not.Contain("Long-term memories:"));
	}

	private class EmptyAIRequester : IAIRequester
	{
		public AIResult GetAIResponse(AIContext context)
		{
			return new AIResult { Response = string.Empty, SessionId = string.Empty };
		}
	}

	private class TestCrudFactory : ICrudFactory
	{
		private readonly Dictionary<Type, object> _cruds = new();

		public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
		{
			if (!_cruds.ContainsKey(typeof(T)))
			{
				_cruds[typeof(T)] = new TestGenericCrud<T>();
			}
			return (ICrud<T>)_cruds[typeof(T)];
		}
	}

	private class TestGenericCrud<T> : ICrud<T> where T : class, IIdentifyable
	{
		private readonly List<T> _items = new();

		public Task<T> CreateAsync(T item)
		{
			_items.Add(item);
			return Task.FromResult(item);
		}

		public Task<T?> GetByIdAsync(string id)
		{
			return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
		}

		public Task<T?> UpdateAsync(string id, T item)
		{
			var index = _items.FindIndex(x => x.Id == id);
			if (index >= 0)
			{
				_items[index] = item;
				return Task.FromResult<T?>(item);
			}
			return Task.FromResult<T?>(null);
		}

		public Task<bool> DeleteAsync(string id)
		{
			var item = _items.FirstOrDefault(x => x.Id == id);
			if (item != null)
			{
				_items.Remove(item);
				return Task.FromResult(true);
			}
			return Task.FromResult(false);
		}

		public Task<T?> GetByQueryAsync(Func<T, bool> query)
		{
			return Task.FromResult(_items.FirstOrDefault(query));
		}

		public Task<IEnumerable<T>> QueryAsync(Func<T, bool> query)
		{
			return Task.FromResult(_items.Where(query));
		}
	}

	private class TestMemoryExtractionService : IMemoryExtractionService
	{
		public Task<MemoryExtractionResponse> ExtractMemoriesAsync(MemoryExtractionRequest request)
		{
			return Task.FromResult(new MemoryExtractionResponse
			{
				Candidates = new[]
				{
					new CandidateMemory
					{
						Summary = "Test memory",
						TypeSuggestion = "LongTerm",
						Confidence = 0.9,
						Tags = new[] { "test" }
					}
				}
			});
		}

		public Task<double> CalculateSimilarityAsync(string summary1, string summary2)
		{
			return Task.FromResult(0.0);
		}

		public Task<IEnumerable<MemoryRecord>> FilterDuplicatesAsync(
			IEnumerable<CandidateMemory> candidates,
			IEnumerable<MemoryRecord> existingMemories,
			double similarityThreshold)
		{
			return Task.FromResult(candidates.Select(c => new MemoryRecord
			{
				Id = Guid.NewGuid().ToString(),
				Summary = c.Summary,
				Type = MemoryType.LongTerm,
				ConfidenceScore = c.Confidence,
				CreatedAt = DateTime.UtcNow
			}));
		}
	}
}
