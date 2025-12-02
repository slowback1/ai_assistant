using System;
using Common.Interfaces;

namespace Common.Models;

public class HumanActivity : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityFrequency Frequency { get; set; } = ActivityFrequency.Daily;
    public DateTime? LastRequested { get; set; }
}

public enum ActivityFrequency
{
    Hourly,
    Daily,
    EveryFewDays,
    Weekly
}
