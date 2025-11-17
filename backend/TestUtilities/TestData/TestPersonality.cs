using Common.Interfaces;

namespace TestUtilities.TestData;

public class TestPersonality(string name, string description) : IPersonality
{
	public string Name { get; } = name;
	public string Description { get; } = description;
}