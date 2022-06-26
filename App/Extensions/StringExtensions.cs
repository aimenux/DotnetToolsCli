using System.Text.Json;

namespace App.Extensions;

public static class StringExtensions
{
    public static bool IgnoreCaseEquals(this string input, string key)
    {
        return input.Equals(key, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IgnoreCaseContains(this string input, string key)
    {
        return input.Contains(key, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetFormattedJson(this string json)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document, options);
    }
}