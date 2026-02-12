using System;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Personality;

public class CreatePersonalityUseCase(ICrud<PersonalityModel> crud)
{
	private readonly ICrud<PersonalityModel> _crud = crud;

	public async Task<UseCaseResult<PersonalityModel>> Execute(PersonalityModel personality)
	{
		if (string.IsNullOrWhiteSpace(personality.Name))
		{
			return UseCaseResult<PersonalityModel>.Failure("Name is required");
		}

		if (string.IsNullOrWhiteSpace(personality.Description))
		{
			return UseCaseResult<PersonalityModel>.Failure("Description is required");
		}

		// Generate a GUID for the personality
		personality.Id = Guid.NewGuid().ToString();

		var created = await _crud.CreateAsync(personality);
		return UseCaseResult<PersonalityModel>.Success(created);
	}
}
