using System.Collections.Generic;
using Common.Interfaces;

namespace Common.Models;

public class Personality : IPersonality, IIdentifyable
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string? Age { get; set; }
	public string? PhysicalFeatures { get; set; }
	public IReadOnlyList<string>? Loves { get; set; }
	public IReadOnlyList<string>? Likes { get; set; }
	public IReadOnlyList<string>? Dislikes { get; set; }
	public IReadOnlyList<string>? Hates { get; set; }
	public IReadOnlyList<string>? Relationships { get; set; }
	public string? TypicalClothing { get; set; }
	public string? DreamsForFuture { get; set; }
	public string? WhatTheyWillTalkAbout { get; set; }
	public string? Occupation { get; set; }
	public string? Background { get; set; }
	public string? Motivations { get; set; }
	public bool IsActive { get; set; }
}
