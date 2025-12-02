using Common.Models;
using InMemory;
using Logic.HumanActivity;

namespace Logic.Tests.HumanActivity;

public class ListHumanActivitiesUseCaseTests
{
    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<Common.Models.HumanActivity>.ClearStaticState();
    }

    [Test]
    public async Task Execute_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new ListHumanActivitiesUseCase(crud);

        var result = await useCase.Execute();

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Empty);
    }

    [Test]
    public async Task Execute_ReturnsAllActivities_WhenActivitiesExist()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        await crud.CreateAsync(new Common.Models.HumanActivity { Name = "Activity 1" });
        await crud.CreateAsync(new Common.Models.HumanActivity { Name = "Activity 2" });
        var useCase = new ListHumanActivitiesUseCase(crud);

        var result = await useCase.Execute();

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result!.Count(), Is.EqualTo(2));
    }
}
