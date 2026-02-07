using NUnit.Framework;
using Common.Models;
using System;

namespace Common.Tests.Models;

public class MemoryRecordTests
{
	[Test]
	public void MemoryRecord_DefaultValues_AreInitialized()
	{
		var memory = new MemoryRecord();

		Assert.That(memory.Id, Is.EqualTo(string.Empty));
		Assert.That(memory.Summary, Is.EqualTo(string.Empty));
		Assert.That(memory.Type, Is.EqualTo(MemoryType.ShortTerm));
		Assert.That(memory.PersonalityId, Is.EqualTo(string.Empty));
		Assert.That(memory.SessionId, Is.Null);
		Assert.That(memory.SourceStoryEventId, Is.EqualTo(string.Empty));
		Assert.That(memory.Tags, Is.Empty);
		Assert.That(memory.ConfidenceScore, Is.EqualTo(0.0));
		Assert.That(memory.EmbeddingVector, Is.Null);
	}

	[Test]
	public void MemoryRecord_CanSetAllProperties()
	{
		var memory = new MemoryRecord
		{
			Id = "mem-123",
			Summary = "Defeated the dragon",
			Type = MemoryType.LongTerm,
			PersonalityId = "vader-1",
			SessionId = "session-456",
			SourceStoryEventId = "event-789",
			Tags = new[] { "achievement", "combat" },
			ConfidenceScore = 0.95,
			EmbeddingVector = new[] { 0.1, 0.2, 0.3 },
			CreatedAt = DateTime.UtcNow
		};

		Assert.That(memory.Id, Is.EqualTo("mem-123"));
		Assert.That(memory.Summary, Is.EqualTo("Defeated the dragon"));
		Assert.That(memory.Type, Is.EqualTo(MemoryType.LongTerm));
		Assert.That(memory.PersonalityId, Is.EqualTo("vader-1"));
		Assert.That(memory.SessionId, Is.EqualTo("session-456"));
		Assert.That(memory.SourceStoryEventId, Is.EqualTo("event-789"));
		Assert.That(memory.Tags, Has.Length.EqualTo(2));
		Assert.That(memory.ConfidenceScore, Is.EqualTo(0.95));
		Assert.That(memory.EmbeddingVector, Has.Length.EqualTo(3));
	}
}
