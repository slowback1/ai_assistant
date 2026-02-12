using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IMemoryExtractionService
{
	Task<MemoryExtractionResponse> ExtractMemoriesAsync(MemoryExtractionRequest request);
	Task<double> CalculateSimilarityAsync(string summary1, string summary2);
	Task<IEnumerable<MemoryRecord>> FilterDuplicatesAsync(
		IEnumerable<CandidateMemory> candidates,
		IEnumerable<MemoryRecord> existingMemories,
		double similarityThreshold);
}
