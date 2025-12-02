using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.HumanActivity;

public class UpdateHumanActivityUseCase(ICrud<Common.Models.HumanActivity> crud)
{
    public async Task<UseCaseResult<Common.Models.HumanActivity>> Execute(string id, Common.Models.HumanActivity activity)
    {
        if (string.IsNullOrWhiteSpace(activity.Name))
        {
            return UseCaseResult<Common.Models.HumanActivity>.Failure("Name is required");
        }

        var existing = await crud.GetByIdAsync(id);
        if (existing == null)
        {
            return UseCaseResult<Common.Models.HumanActivity>.Failure("Activity not found");
        }

        activity.Id = id;
        var updated = await crud.UpdateAsync(id, activity);
        return UseCaseResult<Common.Models.HumanActivity>.Success(updated);
    }
}
