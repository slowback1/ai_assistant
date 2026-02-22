using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donetick;

public class DonetickService
{
    private readonly DonetickClient _client;

    public DonetickService(DonetickClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<List<ChoreDto>> GetOverdueChoresAsync()
    {
        var allChores = await _client.GetAllChoresAsync();
        var now = DateTime.UtcNow;

        return allChores
            .Where(chore => chore.NextDueDate.HasValue && chore.NextDueDate.Value < now)
            .OrderBy(chore => chore.NextDueDate)
            .ToList();
    }

    public async Task<bool> CompleteChoreAsync(int choreId)
    {
        return await _client.CompleteChoreAsync(choreId);
    }
}
