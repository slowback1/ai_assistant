using AIChat;
using CharacterPersonalities;
using Common.Interfaces;
using Common.Models;
using Logic.AI;
using Quartz;

namespace WebAPI.Jobs;

public class StoryGenerationJob : IJob
{
	private readonly ICrudFactory _crudFactory;
	private readonly AIConfig _aiConfig;
	private readonly MemoryConfig _memoryConfig;
	private readonly IMemoryExtractionService? _memoryService;
	private readonly ILogger<StoryGenerationJob> _logger;

	public StoryGenerationJob(
		ICrudFactory crudFactory,
		AIConfig aiConfig,
		MemoryConfig memoryConfig,
		ILogger<StoryGenerationJob> logger,
		IMemoryExtractionService? memoryService = null)
	{
		_crudFactory = crudFactory;
		_aiConfig = aiConfig;
		_memoryConfig = memoryConfig;
		_logger = logger;
		_memoryService = memoryService;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			_logger.LogInformation("Starting story generation job at {Time}", DateTime.UtcNow);

			var storyCrud = _crudFactory.GetCrud<StoryEvent>();
			var personalityCrud = _crudFactory.GetCrud<Personality>();
			
			// Get previous events, ordered by creation time
			var previousEvents = await storyCrud.QueryAsync(_ => true);
			var orderedEvents = previousEvents.OrderBy(e => e.CreatedAt).ToList();

			// Generate new story
			IPersonality? personality = (await personalityCrud.GetByQueryAsync(p => p.IsActive));

			if (personality is null)
			{
				personality = new DarthVader();
			}
			
			// TODO: Implement proper personality ID system. Currently using name as ID.
			// Consider: 1) Adding Id property to IPersonality, or
			//          2) Using stable hash based on personality type name
			var personalityId = personality.Name;
			var requester = new AIChatRequester();
			
			StoryGenerationUseCase useCase;
			if (_memoryConfig.EnableMemoryExtraction && _memoryService != null)
			{
				useCase = new StoryGenerationUseCase(_aiConfig, requester, _crudFactory, _memoryService, _memoryConfig);
			}
			else
			{
				useCase = new StoryGenerationUseCase(_aiConfig, requester);
			}

			var storyEventId = Guid.NewGuid().ToString();
			var result = await useCase.ExecuteAsync(personality, orderedEvents, personalityId, null, storyEventId);

			if (result.Status == UseCaseStatus.Success)
			{
				var storyEvent = new StoryEvent
				{
					Id = storyEventId,
					Story = result.Result!,
					CreatedAt = DateTime.UtcNow
				};

				await storyCrud.CreateAsync(storyEvent);
				_logger.LogInformation("Story generated successfully: {Story}", storyEvent.Story);
				
				if (_memoryConfig.EnableMemoryExtraction)
				{
					_logger.LogInformation("Memory extraction enabled for personality: {PersonalityId}", personalityId);
				}
			}
			else
			{
				_logger.LogError("Failed to generate story: {Error}", result.ErrorMessage);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing story generation job");
		}
	}
}
