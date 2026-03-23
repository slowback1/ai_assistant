using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Activity;

public class GetCurrentActivityUseCase(
	ICrud<Common.Models.Activity> activityCrud,
	ICrud<ActivityPickerState> stateCrud)
{
	public async Task<UseCaseResult<Common.Models.Activity?>> Execute()
	{
		var state = await stateCrud.GetByIdAsync(ActivityPickerState.SingletonId);
		if (state?.CurrentActivityId is null)
			return UseCaseResult<Common.Models.Activity?>.Success(null);

		var activity = await activityCrud.GetByIdAsync(state.CurrentActivityId);
		return UseCaseResult<Common.Models.Activity?>.Success(activity);
	}
}
