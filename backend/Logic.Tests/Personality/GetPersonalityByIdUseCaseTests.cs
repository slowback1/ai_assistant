using Common.Interfaces;
using Common.Models;
using InMemory;
using Logic.Personality;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Tests.Personality;

public class GetPersonalityByIdUseCaseTests
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
	public async Task Execute_WithExistingId_ReturnsSuccess()
	{
		var crud = CreateCrud();
		var personality = new PersonalityModel
		{
			Name = "Test Character",
			Description = "A test character"
		};
		var created = await crud.CreateAsync(personality);
		var useCase = new GetPersonalityByIdUseCase(crud);

		var result = await useCase.Execute(created.Id);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.NotNull(result.Result);
		Assert.That(result.Result!.Id, Is.EqualTo(created.Id));
		Assert.That(result.Result.Name, Is.EqualTo("Test Character"));
	}

	[Test]
	public async Task Execute_WithNonExistingId_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new GetPersonalityByIdUseCase(crud);

		var result = await useCase.Execute("non-existing-id");

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Personality not found"));
	}

	[Test]
	public async Task Execute_WithEmptyId_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new GetPersonalityByIdUseCase(crud);

		var result = await useCase.Execute("");

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Id is required"));
	}

	[Test]
	public async Task Execute_WithNullId_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new GetPersonalityByIdUseCase(crud);

		var result = await useCase.Execute(null!);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Id is required"));
	}
}
