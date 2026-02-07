using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using PersonalityModel = Common.Models.Personality;

namespace Logic.Personality;

public class PaginatePersonalitiesUseCase(ICrud<PersonalityModel> crud)
{
	private readonly ICrud<PersonalityModel> _crud = crud;

	public async Task<UseCaseResult<PaginatedPersonalitiesResult>> Execute(int page = 1, int pageSize = 10, string? nameFilter = null, string? sortBy = "name", string? sortOrder = "asc")
	{
		if (page < 1)
		{
			return UseCaseResult<PaginatedPersonalitiesResult>.Failure("Page must be greater than 0");
		}

		if (pageSize < 1 || pageSize > 100)
		{
			return UseCaseResult<PaginatedPersonalitiesResult>.Failure("Page size must be between 1 and 100");
		}

		// Get all personalities
		var allPersonalities = await _crud.QueryAsync(_ => true);
		var personalitiesList = allPersonalities.ToList();

		// Apply name filter if provided
		if (!string.IsNullOrWhiteSpace(nameFilter))
		{
			personalitiesList = personalitiesList.Where(p => 
				p.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
		}

		// Apply sorting
		personalitiesList = sortBy?.ToLower() switch
		{
			"name" => sortOrder?.ToLower() == "desc" 
				? personalitiesList.OrderByDescending(p => p.Name).ToList()
				: personalitiesList.OrderBy(p => p.Name).ToList(),
			_ => personalitiesList.OrderBy(p => p.Name).ToList()
		};

		// Apply pagination
		var totalCount = personalitiesList.Count;
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
		var skip = (page - 1) * pageSize;
		var paginatedList = personalitiesList.Skip(skip).Take(pageSize).ToList();

		var result = new PaginatedPersonalitiesResult
		{
			Items = paginatedList,
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount,
			TotalPages = totalPages
		};

		return UseCaseResult<PaginatedPersonalitiesResult>.Success(result);
	}
}

public class PaginatedPersonalitiesResult
{
	public List<PersonalityModel> Items { get; set; } = new();
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
	public int TotalPages { get; set; }
}
