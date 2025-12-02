using System.Collections.Generic;

namespace Common.Interfaces;

public interface IPersonality
{
	string Name { get; }
	string Description { get; }
	string? Age { get; }
	string? PhysicalFeatures { get; }
	IReadOnlyList<string>? Loves { get; }
	IReadOnlyList<string>? Likes { get; }
	IReadOnlyList<string>? Dislikes { get; }
	IReadOnlyList<string>? Hates { get; }
	IReadOnlyList<string>? Relationships { get; }
	string? TypicalClothing { get; }
	string? DreamsForFuture { get; }
	string? WhatTheyWillTalkAbout { get; }
	string? Occupation { get; }
	string? Background { get; }
	string? Motivations { get; }
}