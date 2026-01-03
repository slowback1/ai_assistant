using Common.Models;

namespace WebAPI.Integration.Tests.Controllers;

public class HumanActivityControllerTests : ControllerTestBase
{
    [Test]
    public async Task List_ReturnsListOfActivities()
    {
        var activity = new HumanActivity { Name = "List Test", Frequency = ActivityFrequency.Daily };
        await PostAsync<HumanActivity, HumanActivity>("/HumanActivity", activity);

        var response = await GetAsync<HumanActivity[]>("/HumanActivity");

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Any(a => a.Name == "List Test"), Is.True);
    }

    [Test]
    public async Task Create_CreatesAndReturnsActivity()
    {
        var activity = new HumanActivity
        {
            Name = "Test Activity",
            Description = "Test Description",
            Frequency = ActivityFrequency.Daily
        };

        var response = await PostAsync<HumanActivity, HumanActivity>("/HumanActivity", activity);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Name, Is.EqualTo("Test Activity"));
        Assert.That(response.Description, Is.EqualTo("Test Description"));
        Assert.That(response.Id, Is.Not.Empty);
    }

    [Test]
    public async Task Get_ReturnsActivity_WhenActivityExists()
    {
        var activity = new HumanActivity { Name = "Get Test", Frequency = ActivityFrequency.Weekly };
        var created = await PostAsync<HumanActivity, HumanActivity>("/HumanActivity", activity);

        var response = await GetAsync<HumanActivity>($"/HumanActivity/{created!.Id}");

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Name, Is.EqualTo("Get Test"));
    }

    [Test]
    public async Task Update_UpdatesActivity()
    {
        var activity = new HumanActivity { Name = "Original", Frequency = ActivityFrequency.Hourly };
        var created = await PostAsync<HumanActivity, HumanActivity>("/HumanActivity", activity);

        var updated = new HumanActivity
        {
            Name = "Updated",
            Description = "Updated Description",
            Frequency = ActivityFrequency.EveryFewDays
        };
        var response = await PutAsync<HumanActivity, HumanActivity>($"/HumanActivity/{created!.Id}", updated);

        Assert.That(response, Is.Not.Null);

        var fetched = await GetAsync<HumanActivity>($"/HumanActivity/{created.Id}");
        Assert.That(fetched!.Name, Is.EqualTo("Updated"));
        Assert.That(fetched.Description, Is.EqualTo("Updated Description"));
    }

    [Test]
    public async Task Delete_DeletesActivity()
    {
        var activity = new HumanActivity { Name = "To Delete", Frequency = ActivityFrequency.Daily };
        var created = await PostAsync<HumanActivity, HumanActivity>("/HumanActivity", activity);

        await DeleteAsync<bool>($"/HumanActivity/{created!.Id}");

        var response = await GetRawAsync($"/HumanActivity/{created.Id}");
        Assert.That(response.IsSuccessStatusCode, Is.False);
    }
}
