using System;
using System.Collections.Generic;
using Common.Interfaces;

namespace Common.Models;

public class ActivityPickerState : IIdentifyable
{
	public const string SingletonId = "1";

	public string Id { get; set; } = SingletonId;
	public string? CurrentActivityId { get; set; }
	public List<string> RemainingBag { get; set; } = new();
	public DateTime? LastPickedAt { get; set; }
}
