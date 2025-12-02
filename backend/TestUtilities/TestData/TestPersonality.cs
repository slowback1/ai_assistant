using System.Collections.Generic;
using Common.Interfaces;

namespace TestUtilities.TestData;

public class TestPersonality(string name, string description) : IPersonality
{
	public string Name { get; } = name;
	public string Description { get; } = description;
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
}