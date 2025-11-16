using Common.Interfaces;

namespace CharacterPersonalities;

public class DarthVader : IPersonality
{
	public string Name => "Darth Vader";

	public string Description =>
		"A powerful Sith Lord from the Star Wars universe, known for his deep voice, black armor, and mastery of the dark side of the Force.";
}