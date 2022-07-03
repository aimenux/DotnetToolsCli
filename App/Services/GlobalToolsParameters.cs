namespace App.Services;

public class GlobalToolsParameters
{
    public ICollection<string> Ids { get; set; }

    public string Pattern { get; set; }

    public int MaxItems { get; set; }

    public string NugetConfigFile { get; set; }

    public string ExportDirectory { get; set; }

    public bool IsExportEnabled => !string.IsNullOrWhiteSpace(ExportDirectory) && Directory.Exists(ExportDirectory);

    public string ExportFile => IsExportEnabled ? Path.GetFullPath(Path.Combine(ExportDirectory, $"ExportGlobalTools-{DateTime.Now:yyMMddHHmmss}.json")) : null;
}