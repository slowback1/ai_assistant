using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Personality;

public class GetPersonalityByIdUseCase(ICrud<PersonalityModel> crud)
{
	private readonly ICrud<PersonalityModel> _crud = crud;

	public async Task<UseCaseResult<PersonalityModel>> Execute(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return UseCaseResult<PersonalityModel>.Failure("Id is required");
		}

		var personality = await _crud.GetByIdAsync(id);
		
		if (personality == null)
		{
			return UseCaseResult<PersonalityModel>.Failure("Personality not found");
		}

		return UseCaseResult<PersonalityModel>.Success(personality);
	}
}
