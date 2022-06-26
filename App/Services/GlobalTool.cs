using App.Extensions;

namespace App.Services;

public class GlobalTool
{
    public string Id { get; set; }

    public string Version { get; set; }

    public string Command { get; set; }

    public bool IsCurrentTool => string.Equals(Id, Settings.PackageId, StringComparison.OrdinalIgnoreCase);

    public bool IsMatchingPattern(string pattern)
    {
        if (pattern is null) return false;
        return Id.IgnoreCaseContains(pattern) || Command.IgnoreCaseContains(pattern);
    }
}