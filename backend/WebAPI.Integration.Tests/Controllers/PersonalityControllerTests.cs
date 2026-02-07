using Common.Models;
using Logic.Personality;

namespace WebAPI.Integration.Tests.Controllers;

public class PersonalityControllerTests : ControllerTestBase
{
	[Test]
	public async Task CreatePersonality_ReturnsCreatedPersonality()
	{
		var personality = new Personality
		{
			Name = "Test Character",
			Description = "A test character for integration testing"
		};

		var url = "/personality";
		var response = await PostAsync<Personality, Personality>(url, personality);

		Assert.That(response, Is.Not.Null);
		Assert.That(response!.Name, Is.EqualTo("Test Character"));
		Assert.That(response.Description, Is.EqualTo("A test character for integration testing"));
		Assert.That(response.Id, Is.Not.Null);
		Assert.That(response.Id, Is.Not.Empty);
	}

	[Test]
	public async Task GetPersonalityById_ReturnsPersonality()
	{
		// First create a personality
		var personality = new Personality
		{
			Name = "Get Test Character",
			Description = "A test character for get testing"
		};

		var createUrl = "/personality";
		var created = await PostAsync<Personality, Personality>(createUrl, personality);

		// Now get it by id
		var getUrl = $"/personality/{created!.Id}";
		var response = await GetAsync<Personality>(getUrl);

		Assert.That(response, Is.Not.Null);
		Assert.That(response!.Id, Is.EqualTo(created.Id));
		Assert.That(response.Name, Is.EqualTo("Get Test Character"));
	}

	[Test]
	public async Task UpdatePersonality_ReturnsUpdatedPersonality()
	{
		// First create a personality
		var personality = new Personality
		{
			Name = "Original Name",
			Description = "Original Description"
		};

		var createUrl = "/personality";
		var created = await PostAsync<Personality, Personality>(createUrl, personality);

		// Now update it
		var updated = new Personality
		{
			Id = created!.Id,
			Name = "Updated Name",
			Description = "Updated Description"
		};

		var updateUrl = $"/personality/{created.Id}";
		await PutAsync<Personality, Personality>(updateUrl, updated);

		// Verify the update by fetching the personality again
		var getUrl = $"/personality/{created.Id}";
		var response = await GetAsync<Personality>(getUrl);

		Assert.That(response, Is.Not.Null);
		Assert.That(response!.Id, Is.EqualTo(created.Id));
		Assert.That(response.Name, Is.EqualTo("Updated Name"));
		Assert.That(response.Description, Is.EqualTo("Updated Description"));
	}

	[Test]
	public async Task PaginatePersonalities_ReturnsPagedResults()
	{
		// Create multiple personalities
		for (int i = 0; i < 5; i++)
		{
			var personality = new Personality
			{
				Name = $"Character {i}",
				Description = $"Description {i}"
			};
			await PostAsync<Personality, Personality>("/personality", personality);
		}

		// Get paginated results
		var url = "/personality/paginate?page=1&pageSize=3";
		var response = await GetAsync<PaginatedPersonalitiesResult>(url);

		Assert.That(response, Is.Not.Null);
		Assert.That(response!.Items, Has.Count.LessThanOrEqualTo(3));
		Assert.That(response.Page, Is.EqualTo(1));
		Assert.That(response.PageSize, Is.EqualTo(3));
	}
}
