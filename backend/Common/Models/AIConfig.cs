namespace Common.Models;

public class AIConfig
{
	public string Url { get; set; } = string.Empty;
	public string? ApiKey { get; set; } = null;
	public string Model { get; set; } = "deepseek-r1";
}