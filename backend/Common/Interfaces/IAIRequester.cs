using Common.Models;

namespace Common.Interfaces;

public interface IAIRequester
{
	AIResult GetAIResponse(AIContext context);
}