using System;
using Common.Interfaces;

namespace Common.Models;

public class MemoryRecord : IIdentifyable
{
	public string Id { get; set; } = string.Empty;
	public string Summary { get; set; } = string.Empty;
	public MemoryType Type { get; set; }
	public string PersonalityId { get; set; } = string.Empty;
	public string? SessionId { get; set; }
	public string SourceStoryEventId { get; set; } = string.Empty;
	public string[] Tags { get; set; } = Array.Empty<string>();
	public double ConfidenceScore { get; set; }
	public double[]? EmbeddingVector { get; set; }
	public DateTime CreatedAt { get; set; }
}
