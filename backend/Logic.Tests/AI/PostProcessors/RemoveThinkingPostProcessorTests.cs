using Logic.AI.PostProcessors;

namespace Logic.Tests.AI.PostProcessors;

using NUnit.Framework;

public class RemoveThinkingPostProcessorTests
{
    [Test]
    public void RemovesMultipleThinkBlocks()
    {
        var input = "First <think>secret</think> Second <think>hidden</think> Third";
        var processor = new RemoveThinkingPostProcessor();

        var result = processor.PostProcess(input);

        Assert.That(result, Is.EqualTo("First  Second  Third".Trim()));
    }

    [Test]
    public void StrayClosingTag_RemovesEverythingUpToAndIncludingClosing()
    {
        var input = "garbage data </think>Actual content";
        var processor = new RemoveThinkingPostProcessor();

        var result = processor.PostProcess(input);

        Assert.That(result, Is.EqualTo("Actual content"));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void NullOrEmpty_ReturnsEmptyString(string? nullishInput)
    {
        var processor = new RemoveThinkingPostProcessor();

        var nullResult = processor.PostProcess(nullishInput!);
        Assert.That(nullResult, Is.EqualTo(string.Empty));

        var emptyResult = processor.PostProcess(string.Empty);
        Assert.That(emptyResult, Is.EqualTo(string.Empty));
    }

    [Test]
    public void NoThinkTags_ReturnsUnchanged()
    {
        var input = "Just a normal response.";
        var processor = new RemoveThinkingPostProcessor();

        var result = processor.PostProcess(input);

        Assert.That(result, Is.EqualTo(input));
    }
}