using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.HumanActivity;

public class ListHumanActivitiesUseCase(ICrud<Common.Models.HumanActivity> crud)
{
    public async Task<UseCaseResult<IEnumerable<Common.Models.HumanActivity>>> Execute()
    {
        var activities = await crud.QueryAsync(_ => true);
        return UseCaseResult<IEnumerable<Common.Models.HumanActivity>>.Success(activities);
    }
}
