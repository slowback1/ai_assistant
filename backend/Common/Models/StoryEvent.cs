using System;
using Common.Interfaces;

namespace Common.Models;

public class StoryEvent : IIdentifyable
{
	public string Id { get; set; } = string.Empty;
	public string Story { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}
