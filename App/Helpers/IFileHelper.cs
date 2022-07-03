using App.Services;

namespace App.Helpers;

public interface IFileHelper
{
    Task<ICollection<string>> ExtractGlobalToolsIdsAsync(ICollection<string> ids, CancellationToken cancellationToken = default);
    Task ExportGlobalToolsAsync(ICollection<GlobalTool> globalTools, GlobalToolsParameters parameters, CancellationToken cancellationToken = default);
}