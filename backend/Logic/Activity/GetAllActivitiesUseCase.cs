using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Activity;

public class GetAllActivitiesUseCase(ICrud<Common.Models.Activity> crud)
{
	public async Task<UseCaseResult<IEnumerable<Common.Models.Activity>>> Execute()
	{
		var activities = await crud.QueryAsync(_ => true);
		return UseCaseResult<IEnumerable<Common.Models.Activity>>.Success(activities);
	}
}
