using Common.Models;
using InMemory;
using Logic.HumanActivity;

namespace Logic.Tests.HumanActivity;

public class UpdateHumanActivityUseCaseTests
{
    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<Common.Models.HumanActivity>.ClearStaticState();
    }

    [Test]
    public async Task Execute_UpdatesActivity_WhenActivityExists()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var created = await crud.CreateAsync(new Common.Models.HumanActivity { Name = "Original" });
        var useCase = new UpdateHumanActivityUseCase(crud);
        var updated = new Common.Models.HumanActivity
        {
            Name = "Updated",
            Description = "New description",
            Frequency = ActivityFrequency.Weekly
        };

        var result = await useCase.Execute(created.Id, updated);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));

        var fetched = await crud.GetByIdAsync(created.Id);
        Assert.That(fetched!.Name, Is.EqualTo("Updated"));
        Assert.That(fetched.Description, Is.EqualTo("New description"));
        Assert.That(fetched.Frequency, Is.EqualTo(ActivityFrequency.Weekly));
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenActivityDoesNotExist()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new UpdateHumanActivityUseCase(crud);
        var activity = new Common.Models.HumanActivity { Name = "Updated" };

        var result = await useCase.Execute("non-existent-id", activity);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo("Activity not found"));
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenNameIsEmpty()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var created = await crud.CreateAsync(new Common.Models.HumanActivity { Name = "Original" });
        var useCase = new UpdateHumanActivityUseCase(crud);
        var activity = new Common.Models.HumanActivity { Name = "" };

        var result = await useCase.Execute(created.Id, activity);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo("Name is required"));
    }
}
