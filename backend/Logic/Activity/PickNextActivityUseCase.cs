using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Activity;

public class PickNextActivityUseCase(
	ICrud<Common.Models.Activity> activityCrud,
	ICrud<ActivityPickerState> stateCrud)
{
	public async Task<UseCaseResult<Common.Models.Activity?>> Execute()
	{
		var allActivities = (await activityCrud.QueryAsync(_ => true)).ToList();

		// No-op: nothing to pick from
		if (allActivities.Count == 0)
			return UseCaseResult<Common.Models.Activity?>.Success(null);

		// Load or create the singleton state record
		var state = await stateCrud.GetByIdAsync(ActivityPickerState.SingletonId);
		if (state is null)
		{
			state = new ActivityPickerState();
			state = await stateCrud.CreateAsync(state);
		}

		// Refill the bag if it is empty (start a new cycle)
		if (state.RemainingBag.Count == 0)
		{
			var allIds = allActivities.Select(a => a.Id).ToList();
			state.RemainingBag = Shuffle(allIds);
		}

		// Pop the first item from the bag
		var pickedId = state.RemainingBag[0];
		state.RemainingBag.RemoveAt(0);
		state.CurrentActivityId = pickedId;
		state.LastPickedAt = DateTime.UtcNow;

		await stateCrud.UpdateAsync(ActivityPickerState.SingletonId, state);

		var picked = allActivities.FirstOrDefault(a => a.Id == pickedId);
		return UseCaseResult<Common.Models.Activity?>.Success(picked);
	}

	/// <summary>
	/// Fisher-Yates shuffle.
	/// </summary>
	private static List<string> Shuffle(List<string> source)
	{
		var list = new List<string>(source);
		var rng = new Random();
		for (var i = list.Count - 1; i > 0; i--)
		{
			var j = rng.Next(i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
		return list;
	}
}
