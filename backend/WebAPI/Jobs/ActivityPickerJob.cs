using Common.Interfaces;
using Common.Models;
using Logic.Activity;
using Quartz;

namespace WebAPI.Jobs;

public class ActivityPickerJob : IJob
{
	private readonly ICrudFactory _crudFactory;
	private readonly ILogger<ActivityPickerJob> _logger;

	public ActivityPickerJob(ICrudFactory crudFactory, ILogger<ActivityPickerJob> logger)
	{
		_crudFactory = crudFactory;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			_logger.LogInformation("Starting activity picker job at {Time}", DateTime.UtcNow);

			var activityCrud = _crudFactory.GetCrud<Common.Models.Activity>();
			var stateCrud = _crudFactory.GetCrud<ActivityPickerState>();

			var useCase = new PickNextActivityUseCase(activityCrud, stateCrud);
			var result = await useCase.Execute();

			if (result.Status == UseCaseStatus.Success)
			{
				if (result.Result is not null)
					_logger.LogInformation("Activity picked: {ActivityName} ({ActivityId})", result.Result.Name, result.Result.Id);
				else
					_logger.LogInformation("No activities defined — skipping pick");
			}
			else
			{
				_logger.LogError("Failed to pick activity: {Error}", result.ErrorMessage);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing activity picker job");
		}
	}
}
