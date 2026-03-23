using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Activity;

public class CreateActivityUseCase(
	ICrud<Common.Models.Activity> activityCrud,
	ICrud<ActivityPickerState> stateCrud)
{
	public async Task<UseCaseResult<Common.Models.Activity>> Execute(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return UseCaseResult<Common.Models.Activity>.Failure("Name is required");

		var activity = new Common.Models.Activity
		{
			Id = Guid.NewGuid().ToString(),
			Name = name.Trim()
		};

		var created = await activityCrud.CreateAsync(activity);

		// Reset the bag so the new activity is included in the next cycle
		await ResetBagState(stateCrud);

		return UseCaseResult<Common.Models.Activity>.Success(created);
	}

	private static async Task ResetBagState(ICrud<ActivityPickerState> stateCrud)
	{
		var existing = await stateCrud.GetByIdAsync(ActivityPickerState.SingletonId);
		if (existing is not null)
		{
			existing.RemainingBag = new List<string>();
			await stateCrud.UpdateAsync(ActivityPickerState.SingletonId, existing);
		}
	}
}
