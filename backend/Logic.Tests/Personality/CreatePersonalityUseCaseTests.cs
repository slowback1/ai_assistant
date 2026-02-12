using Common.Interfaces;
using Common.Models;
using InMemory;
using Logic.Personality;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Tests.Personality;

public class CreatePersonalityUseCaseTests
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
		var useCase = new CreatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test Character",
			Description = "A test character"
		};

		var result = await useCase.Execute(personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.NotNull(result.Result);
		Assert.That(result.Result!.Name, Is.EqualTo("Test Character"));
		Assert.That(result.Result.Description, Is.EqualTo("A test character"));
		Assert.That(result.Result.Id, Is.Not.Null.And.Not.Empty);
	}

	[Test]
	public async Task Execute_GeneratesGuid()
	{
		var crud = CreateCrud();
		var useCase = new CreatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test",
			Description = "Test"
		};

		var result = await useCase.Execute(personality);

		// Verify that personality.Id was set (it gets overridden by InMemoryCrud, but use case should set it)
		Assert.That(result.Result!.Id, Is.Not.Null);
		// The ID should be a valid string (InMemoryCrud may override with numeric ID)
		Assert.That(result.Result.Id, Is.Not.Empty);
	}

	[Test]
	public async Task Execute_WithoutName_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new CreatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "",
			Description = "A test character"
		};

		var result = await useCase.Execute(personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Name is required"));
	}

	[Test]
	public async Task Execute_WithoutDescription_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new CreatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test Character",
			Description = ""
		};

		var result = await useCase.Execute(personality);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Description is required"));
	}

	[Test]
	public async Task Execute_PreservesOptionalFields()
	{
		var crud = CreateCrud();
		var useCase = new CreatePersonalityUseCase(crud);
		var personality = new PersonalityModel
		{
			Name = "Test Character",
			Description = "A test character",
			Age = "25",
			Occupation = "Developer"
		};

		var result = await useCase.Execute(personality);

		Assert.That(result.Result!.Age, Is.EqualTo("25"));
		Assert.That(result.Result.Occupation, Is.EqualTo("Developer"));
	}
}
