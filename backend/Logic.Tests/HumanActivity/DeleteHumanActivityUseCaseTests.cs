using Common.Models;
using InMemory;
using Logic.HumanActivity;

namespace Logic.Tests.HumanActivity;

public class DeleteHumanActivityUseCaseTests
{
    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<Common.Models.HumanActivity>.ClearStaticState();
    }

    [Test]
    public async Task Execute_DeletesActivity_WhenActivityExists()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var created = await crud.CreateAsync(new Common.Models.HumanActivity { Name = "To Delete" });
        var useCase = new DeleteHumanActivityUseCase(crud);

        var result = await useCase.Execute(created.Id);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.True);

        var fetched = await crud.GetByIdAsync(created.Id);
        Assert.That(fetched, Is.Null);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenActivityDoesNotExist()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new DeleteHumanActivityUseCase(crud);

        var result = await useCase.Execute("non-existent-id");

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo("Activity not found"));
    }
}
