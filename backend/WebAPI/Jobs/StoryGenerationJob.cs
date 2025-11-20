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
	private readonly ILogger<StoryGenerationJob> _logger;

	public StoryGenerationJob(ICrudFactory crudFactory, AIConfig aiConfig, ILogger<StoryGenerationJob> logger)
	{
		_crudFactory = crudFactory;
		_aiConfig = aiConfig;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			_logger.LogInformation("Starting story generation job at {Time}", DateTime.UtcNow);

			var storyCrud = _crudFactory.GetCrud<StoryEvent>();
			
			// Get previous events, ordered by creation time
			var previousEvents = await storyCrud.QueryAsync(_ => true);
			var orderedEvents = previousEvents.OrderBy(e => e.CreatedAt).ToList();

			// Generate new story
			var personality = new DarthVader();
			var requester = new AIChatRequester();
			var useCase = new StoryGenerationUseCase(_aiConfig, requester);

			var result = useCase.Execute(personality, orderedEvents);

			if (result.Status == UseCaseStatus.Success)
			{
				var storyEvent = new StoryEvent
				{
					Id = Guid.NewGuid().ToString(),
					Story = result.Result!,
					CreatedAt = DateTime.UtcNow
				};

				await storyCrud.CreateAsync(storyEvent);
				_logger.LogInformation("Story generated successfully: {Story}", storyEvent.Story);
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
