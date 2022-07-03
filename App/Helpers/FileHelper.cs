using System.Text.Json;
using App.Services;
using Microsoft.Extensions.Logging;

namespace App.Helpers;

public class FileHelper : IFileHelper
{
    private readonly ILogger _logger;

    public FileHelper(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<string>> ExtractGlobalToolsIdsAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        var results = new List<string>();

        foreach (var id in ids.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            if (!File.Exists(id))
            {
                results.Add(id);
                continue;
            }

            try
            {
                var json = await File.ReadAllTextAsync(id, cancellationToken);
                var tools = JsonSerializer.Deserialize<List<GlobalTool>>(json);
                if (tools?.Any() == true)
                {
                    results.AddRange(tools.Select(x => x.Id));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to extract tools from export file '{id}' caused by exception '{ex}'", id, ex.Message);
            }
        }

        return results;
    }

    public async Task ExportGlobalToolsAsync(ICollection<GlobalTool> globalTools, GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        if (!parameters.IsExportEnabled) return;
        var filename = parameters.ExportFile;
        var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(globalTools, jsonSerializerOptions);
        await File.WriteAllTextAsync(filename, json, cancellationToken);
    }
}