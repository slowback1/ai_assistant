using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.HumanActivity;

public class GetHumanActivityUseCase(ICrud<Common.Models.HumanActivity> crud)
{
    public async Task<UseCaseResult<Common.Models.HumanActivity>> Execute(string id)
    {
        var activity = await crud.GetByIdAsync(id);
        if (activity == null)
        {
            return UseCaseResult<Common.Models.HumanActivity>.Failure("Activity not found");
        }
        return UseCaseResult<Common.Models.HumanActivity>.Success(activity);
    }
}
