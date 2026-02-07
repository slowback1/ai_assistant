namespace Common.Models;

public class MemoryExtractionRequest
{
	public string PersonalityProfile { get; set; } = string.Empty;
	public string[] RecentEvents { get; set; } = System.Array.Empty<string>();
	public string StoryOutput { get; set; } = string.Empty;
	public string[] ExistingMemories { get; set; } = System.Array.Empty<string>();
}
