using Common.Interfaces;
using Common.Models;
using InMemory;
using Logic.Personality;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Tests.Personality;

public class PaginatePersonalitiesUseCaseTests
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
	public async Task Execute_WithNoPersonalities_ReturnsEmptyList()
	{
		var crud = CreateCrud();
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute();

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
		Assert.That(result.Result!.Items, Is.Empty);
		Assert.That(result.Result.TotalCount, Is.EqualTo(0));
		Assert.That(result.Result.TotalPages, Is.EqualTo(0));
	}

	[Test]
	public async Task Execute_ReturnsPersonalitiesSortedByName()
	{
		var crud = CreateCrud();
		await crud.CreateAsync(new PersonalityModel { Name = "Charlie", Description = "C" });
		await crud.CreateAsync(new PersonalityModel { Name = "Alice", Description = "A" });
		await crud.CreateAsync(new PersonalityModel { Name = "Bob", Description = "B" });
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute();

		Assert.That(result.Result!.Items[0].Name, Is.EqualTo("Alice"));
		Assert.That(result.Result.Items[1].Name, Is.EqualTo("Bob"));
		Assert.That(result.Result.Items[2].Name, Is.EqualTo("Charlie"));
	}

	[Test]
	public async Task Execute_WithPageSize_ReturnsCorrectNumberOfItems()
	{
		var crud = CreateCrud();
		for (int i = 0; i < 15; i++)
		{
			await crud.CreateAsync(new PersonalityModel { Name = $"Character {i}", Description = "Test" });
		}
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(page: 1, pageSize: 5);

		Assert.That(result.Result!.Items.Count, Is.EqualTo(5));
		Assert.That(result.Result.TotalCount, Is.EqualTo(15));
		Assert.That(result.Result.TotalPages, Is.EqualTo(3));
	}

	[Test]
	public async Task Execute_WithPage2_ReturnsCorrectItems()
	{
		var crud = CreateCrud();
		for (int i = 1; i <= 15; i++)
		{
			await crud.CreateAsync(new PersonalityModel { Name = $"Character {i:D2}", Description = "Test" });
		}
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(page: 2, pageSize: 5);

		Assert.That(result.Result!.Items.Count, Is.EqualTo(5));
		Assert.That(result.Result.Page, Is.EqualTo(2));
	}

	[Test]
	public async Task Execute_WithNameFilter_ReturnsFilteredResults()
	{
		var crud = CreateCrud();
		await crud.CreateAsync(new PersonalityModel { Name = "Alice", Description = "A" });
		await crud.CreateAsync(new PersonalityModel { Name = "Bob", Description = "B" });
		await crud.CreateAsync(new PersonalityModel { Name = "Alice Smith", Description = "C" });
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(nameFilter: "Alice");

		Assert.That(result.Result!.Items.Count, Is.EqualTo(2));
		Assert.That(result.Result.Items[0].Name, Does.Contain("Alice"));
		Assert.That(result.Result.Items[1].Name, Does.Contain("Alice"));
	}

	[Test]
	public async Task Execute_WithDescendingSort_ReturnsSortedDescending()
	{
		var crud = CreateCrud();
		await crud.CreateAsync(new PersonalityModel { Name = "Alice", Description = "A" });
		await crud.CreateAsync(new PersonalityModel { Name = "Bob", Description = "B" });
		await crud.CreateAsync(new PersonalityModel { Name = "Charlie", Description = "C" });
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(sortOrder: "desc");

		Assert.That(result.Result!.Items[0].Name, Is.EqualTo("Charlie"));
		Assert.That(result.Result.Items[1].Name, Is.EqualTo("Bob"));
		Assert.That(result.Result.Items[2].Name, Is.EqualTo("Alice"));
	}

	[Test]
	public async Task Execute_WithInvalidPage_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(page: 0);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Page must be greater than 0"));
	}

	[Test]
	public async Task Execute_WithInvalidPageSize_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(pageSize: 0);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Page size must be between 1 and 100"));
	}

	[Test]
	public async Task Execute_WithPageSizeTooLarge_ReturnsFailure()
	{
		var crud = CreateCrud();
		var useCase = new PaginatePersonalitiesUseCase(crud);

		var result = await useCase.Execute(pageSize: 101);

		Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
		Assert.That(result.ErrorMessage, Is.EqualTo("Page size must be between 1 and 100"));
	}
}
