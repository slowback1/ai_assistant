using Common.Interfaces;
using Common.Models;
using InMemory;
using Logic.Personality;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Tests.Personality;

public class UpdatePersonalityUseCaseTests
{
	[SetUp]
	public void SetUp()
	{
		InMemoryCrud<PersonalityModel>.ClearStaticState();
	}

	private ICrud<PersonalityModel> CreateCrud()
	{
		return new InMemoryGenericCrud<PersonalityModel>();
	}

	[Test]
	public async Task Execute_WithValidPersonality_ReturnsSuccess()
	{
		var crud = CreateCrud();
		var personality = new PersonalityModel
		{
			Name = "Original Name",
			Description = "Original Description"
		};
		var created = await crud.CreateAsync(personality);
		var useCase = new UpdatePersonalityUseCase(crud);

		var updated = new PersonalityModel
		{
			Name = "Updated Name",
			Description = "Updated Description"
		};

		var result = await useCase.Execute(created.Id, updated);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.NotNull(result.Result);
		Assert.That(result.Result!.Id, Is.EqualTo(created.Id));
	}

	[Test]
	public async Task Execute_UpdatesPersonalityInStore()
	{
		var crud = CreateCrud();
		var personality = new PersonalityModel
		{
			Name = "Original Name",
			Description = "Original Description"
		};
		var created = await crud.CreateAsync(personality);
		var useCase = new UpdatePersonalityUseCase(crud);

		var updated = new PersonalityModel
		{
			Name = "Updated Name",
			Description = "Updated Description"
		};

		await useCase.Execute(created.Id, updated);
		var retrieved = await crud.GetByIdAsync(created.Id);

		Assert.That(retrieved!.Name, Is.EqualTo("Updated Name"));
		Assert.That(retrieved.Description, Is.EqualTo("Updated Description"));
	}

	[Test]
	public async Task Execute_WithNonExistingId_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new UpdatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test",
			Description = "Test"
		};

		var result = await useCase.Execute("non-existing-id", personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Personality not found"));
	}

	[Test]
	public async Task Execute_WithoutName_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new UpdatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "",
			Description = "Test"
		};

		var result = await useCase.Execute("some-id", personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Name is required"));
	}

	[Test]
	public async Task Execute_WithoutDescription_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new UpdatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test",
			Description = ""
		};

		var result = await useCase.Execute("some-id", personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Description is required"));
	}

	[Test]
	public async Task Execute_SetsIdCorrectly()
	{
		var crud = CreateCrud();
		var personality = new PersonalityModel
		{
			Name = "Original",
			Description = "Original"
		};
		var created = await crud.CreateAsync(personality);
		var useCase = new UpdatePersonalityUseCase(crud);

		var updated = new PersonalityModel
		{
			Name = "Updated",
			Description = "Updated"
		};

		var result = await useCase.Execute(created.Id, updated);

		Assert.That(result.Result!.Id, Is.EqualTo(created.Id));
	}
}
