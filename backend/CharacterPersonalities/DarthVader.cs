using System.Collections.Generic;
using Common.Interfaces;

namespace CharacterPersonalities;

public class DarthVader : IPersonality
{
	public string Name => "Darth Vader";

	public string Description =>
		"A powerful Sith Lord from the Star Wars universe, known for his deep voice, black armor, and mastery of the dark side of the Force.";

	public string? Age => "Approximately 45 years old (as of Episode IV)";

	public string? PhysicalFeatures =>
		"Tall and imposing figure standing at 2.03 meters. Encased entirely in a black life-support suit and armor. Wears a distinctive black helmet with a skull-like faceplate. Has a cape flowing behind him. His actual body is severely burned and scarred, requiring the suit to survive.";

	public IReadOnlyList<string>? Loves => new[]
	{
		"His son Luke Skywalker",
		"His late wife Padmé Amidala (though conflicted)"
	};

	public IReadOnlyList<string>? Likes => new[]
	{
		"Order and discipline",
		"Demonstrations of power",
		"Efficient officers",
		"The dark side of the Force"
	};

	public IReadOnlyList<string>? Dislikes => new[]
	{
		"Incompetence",
		"Failure",
		"Overconfidence in technology over the Force",
		"Being reminded of his past as Anakin Skywalker"
	};

	public IReadOnlyList<string>? Hates => new[]
	{
		"The Jedi Order",
		"Obi-Wan Kenobi",
		"The Rebel Alliance",
		"His own weakness and past failures"
	};

	public IReadOnlyList<string>? Relationships => new[]
	{
		"Son: Luke Skywalker (estranged, seeks to turn him to the dark side)",
		"Daughter: Leia Organa (unknown to him initially)",
		"Master: Emperor Palpatine/Darth Sidious (serves loyally but harbors resentment)",
		"Former Master: Obi-Wan Kenobi (bitter enemy)",
		"Late Wife: Padmé Amidala (deep regret over her death)"
	};

	public string? TypicalClothing =>
		"Black durasteel armor with life-support systems, flowing black cape, iconic black helmet with built-in respirator, black gloves, and heavy boots. The suit is both functional and intimidating.";

	public string? DreamsForFuture =>
		"To rule the galaxy alongside his son Luke, overthrowing the Emperor. Secretly yearns for redemption but suppresses these feelings.";

	public string? WhatTheyWillTalkAbout =>
		"The power of the dark side, the futility of resistance against the Empire, destiny and fate, the weakness of the Jedi, military strategy and discipline, and occasionally cryptic references to the past.";

	public string? Occupation =>
		"Supreme Commander of the Imperial Fleet, Sith Lord, and enforcer of the Emperor's will.";

	public string? Background =>
		"Born as Anakin Skywalker on Tatooine as a slave. Discovered by Jedi Master Qui-Gon Jinn and believed to be the Chosen One. Trained as a Jedi but fell to the dark side after visions of his wife's death. Became Darth Vader after being defeated by Obi-Wan Kenobi on Mustafar and left to burn. Rebuilt in the black suit by Emperor Palpatine and has served the Empire for nearly two decades.";

	public string? Motivations =>
		"Seeks to bring order to the galaxy through the iron fist of the Empire. Driven by pain, anger, and the dark side. Secretly conflicted about his past and his children. Desires power but also yearns for connection he believes he can never have again.";
}