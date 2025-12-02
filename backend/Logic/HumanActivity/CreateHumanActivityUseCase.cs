using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.HumanActivity;

public class CreateHumanActivityUseCase(ICrud<Common.Models.HumanActivity> crud)
{
    public async Task<UseCaseResult<Common.Models.HumanActivity>> Execute(Common.Models.HumanActivity activity)
    {
        if (string.IsNullOrWhiteSpace(activity.Name))
        {
            return UseCaseResult<Common.Models.HumanActivity>.Failure("Name is required");
        }

        var created = await crud.CreateAsync(activity);
        return UseCaseResult<Common.Models.HumanActivity>.Success(created);
    }
}
