using Common.Models;
using InMemory;
using Logic.HumanActivity;

namespace Logic.Tests.HumanActivity;

public class CreateHumanActivityUseCaseTests
{
    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<Common.Models.HumanActivity>.ClearStaticState();
    }

    [Test]
    public async Task Execute_CreatesActivity_WhenNameIsProvided()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new CreateHumanActivityUseCase(crud);
        var activity = new Common.Models.HumanActivity
        {
            Name = "New Activity",
            Description = "A description",
            Frequency = ActivityFrequency.Daily
        };

        var result = await useCase.Execute(activity);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result!.Name, Is.EqualTo("New Activity"));
        Assert.That(result.Result.Id, Is.Not.Empty);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenNameIsEmpty()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new CreateHumanActivityUseCase(crud);
        var activity = new Common.Models.HumanActivity { Name = "" };

        var result = await useCase.Execute(activity);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo("Name is required"));
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenNameIsWhitespace()
    {
        var crud = new InMemoryGenericCrud<Common.Models.HumanActivity>();
        var useCase = new CreateHumanActivityUseCase(crud);
        var activity = new Common.Models.HumanActivity { Name = "   " };

        var result = await useCase.Execute(activity);

        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo("Name is required"));
    }
}
