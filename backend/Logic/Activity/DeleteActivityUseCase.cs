using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Activity;

public class DeleteActivityUseCase(
	ICrud<Common.Models.Activity> activityCrud,
	ICrud<ActivityPickerState> stateCrud)
{
	public async Task<UseCaseResult<bool>> Execute(string id)
	{
		var activity = await activityCrud.GetByIdAsync(id);
		if (activity is null)
			return UseCaseResult<bool>.Failure("Activity not found");

		await activityCrud.DeleteAsync(id);

		// Reset the bag so the deleted activity is no longer in rotation
		var state = await stateCrud.GetByIdAsync(ActivityPickerState.SingletonId);
		if (state is not null)
		{
			state.RemainingBag = new List<string>();
			// If the deleted activity was the current one, clear it
			if (state.CurrentActivityId == id)
				state.CurrentActivityId = null;
			await stateCrud.UpdateAsync(ActivityPickerState.SingletonId, state);
		}

		return UseCaseResult<bool>.Success(true);
	}
}
