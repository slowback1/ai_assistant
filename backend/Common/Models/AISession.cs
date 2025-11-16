namespace Common.Models;

public class AISession
{
	public string SessionId { get; set; } = string.Empty;
	public AiMessage[] Messages { get; set; } = [];
}

public struct AiMessage
{
	public string Message { get; set; }
	public string Role { get; set; }
}