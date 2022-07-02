namespace App.Services;

public interface IGlobalToolService
{
    public Task<ICollection<GlobalTool>> GetGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default);

    public Task<ICollection<GlobalTool>> SearchGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default);

    public Task<ICollection<GlobalTool>> InstallGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default);

    public Task<ICollection<GlobalTool>> UninstallGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default);

    public Task<(ICollection<GlobalTool>, ICollection<GlobalTool>)> UpdateGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default);
}