using Common.Interfaces;
using Common.Models;

namespace TestUtilities.TestData;

public class TestAIRequester : IAIRequester
{
	public AIResult GetAIResponse(AIContext context)
	{
		return new AIResult
		{
			Response = $"Echo: {context.Message}",
			SessionId = context.Session?.SessionId ?? string.Empty
		};
	}
}