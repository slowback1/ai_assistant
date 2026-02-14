using System;

namespace Donetick;

public class ChoreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? NextDueDate { get; set; }
}
