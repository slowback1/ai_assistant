namespace Common.Models;

public class MemoryConfig
{
	public bool EnableMemoryExtraction { get; set; } = false;
	public double ConfidenceThreshold { get; set; } = 0.7;
	public double SimilarityThreshold { get; set; } = 0.85;
	public int MaxMemoriesPerPrompt { get; set; } = 10;
	public int ShortTermMemoryCount { get; set; } = 5;
	public int MemoryTtlDays { get; set; } = 180;
}
