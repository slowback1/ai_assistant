using System;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Personality;

public class UpdatePersonalityUseCase(ICrud<PersonalityModel> crud)
{
	private readonly ICrud<PersonalityModel> _crud = crud;

	public async Task<UseCaseResult<PersonalityModel>> Execute(string id, PersonalityModel personality)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return UseCaseResult<PersonalityModel>.Failure("Id is required");
		}

		if (string.IsNullOrWhiteSpace(personality.Name))
		{
			return UseCaseResult<PersonalityModel>.Failure("Name is required");
		}

		if (string.IsNullOrWhiteSpace(personality.Description))
		{
			return UseCaseResult<PersonalityModel>.Failure("Description is required");
		}

		// Ensure the Id is set correctly
		personality.Id = id;

		var updated = await _crud.UpdateAsync(id, personality);
		
		if (updated == null)
		{
			return UseCaseResult<PersonalityModel>.Failure("Personality not found");
		}

		return UseCaseResult<PersonalityModel>.Success(updated);
	}
}
