using System;
using System.Linq;

namespace Common.Models;

public class AIRequest
{
	public AISession? Session { get; set; }
	public string? SessionId => Session?.SessionId;
	public string Message { get; set; } = string.Empty;
}

public class AIContext : AIRequest
{
	public AIConfig Config { get; set; } = new();

	public void AddSessionMessage(string message, string role)
	{
		if (Session == null)
		{
			Session = new AISession();
			Session.SessionId = Guid.NewGuid().ToString();
		}

		var messageList = Session.Messages.ToList();
		messageList.Add(new AiMessage { Message = message, Role = role });
		Session.Messages = messageList.ToArray();
	}
}