namespace Logic.AI.PostProcessors;

using System;
using System.Text.RegularExpressions;

public class RemoveThinkingPostProcessor : IAIPostProcessor
{
    public string PostProcess(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse)) return string.Empty;

        var result = aiResponse;

        // Remove any <think>...</think> blocks 
        result = Regex.Replace(result, @"<\s*think\b[^>]*>.*?<\s*/\s*think\s*>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        var closeTag = "</think>";
        var idx = result.IndexOf(closeTag, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            result = result.Substring(idx + closeTag.Length);
        }

        return result.Trim();
    }
}