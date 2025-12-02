using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.HumanActivity;

public class DeleteHumanActivityUseCase(ICrud<Common.Models.HumanActivity> crud)
{
    public async Task<UseCaseResult<bool>> Execute(string id)
    {
        var existing = await crud.GetByIdAsync(id);
        if (existing == null)
        {
            return UseCaseResult<bool>.Failure("Activity not found");
        }

        await crud.DeleteAsync(id);
        return UseCaseResult<bool>.Success(true);
    }
}
