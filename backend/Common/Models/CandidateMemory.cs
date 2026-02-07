namespace Common.Models;

public class CandidateMemory
{
	public string Summary { get; set; } = string.Empty;
	public string TypeSuggestion { get; set; } = string.Empty;
	public double Confidence { get; set; }
	public string[] Tags { get; set; } = System.Array.Empty<string>();
	public string Reasoning { get; set; } = string.Empty;
}
