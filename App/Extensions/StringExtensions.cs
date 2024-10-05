using System.Text.Json;

namespace App.Extensions;

public static class StringExtensions
{
    public static bool IgnoreCaseEquals(this string input, string key)
    {
        return string.Equals(input, key, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IgnoreCaseContains(this string input, string key)
    {
        if (input is null || key is null) return input == key;
        return input.Contains(key, StringComparison.OrdinalIgnoreCase) == true;
    }
    
    public static bool IgnoreCaseContains(this string input, string[] keys)
    {
        if (input is null && keys is null) return true;
        if (input is null) return false;
        if (keys is null) return false;
        return keys.All(input.IgnoreCaseContains);
    }

    public static string GetFormattedJson(this string json)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document, options);
    }
}