namespace Common.Models;

public class AIContext
{
	public AIConfig Config { get; set; } = new();
	public string? SessionId { get; set; }
	public string Message { get; set; } = string.Empty;
}