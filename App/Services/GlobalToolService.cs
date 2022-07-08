using System.Collections.Concurrent;
using App.Extensions;
using App.Helpers;
using Microsoft.Extensions.Logging;

namespace App.Services;

public class GlobalToolService : IGlobalToolService
{
    private readonly IProcessHelper _processHelper;
    private readonly IRandomHelper _randomHelper;
    private readonly ILogger _logger;

    public GlobalToolService(IProcessHelper processHelper, IRandomHelper randomHelper, ILogger logger)
    {
        _processHelper = processHelper ?? throw new ArgumentNullException(nameof(processHelper));
        _randomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<GlobalTool>> GetGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        const string name = @"dotnet";
        const string arguments = @"tool list -g";
        var queue = new ConcurrentQueue<GlobalTool>();
        await _processHelper.RunProcessAsync(name, arguments, (_, args) =>
        {
            var message = args.Data;
            var globalTool = ExtractGlobalTool(message);
            if (globalTool == null) return;
            queue.Enqueue(globalTool);
        }, cancellationToken);

        var pattern = parameters.Pattern;
        var globalTools = queue
            .Where(x => pattern is null || x.IsMatchingPattern(pattern))
            .OrderBy(x => x.Id)
            .ToList();
        return globalTools;
    }

    public async Task<ICollection<GlobalTool>> SearchGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken = default)
    {
        const int size = 20;
        const string name = @"dotnet";
        var max = parameters.MaxItems;
        var pattern = parameters.Pattern;
        var cache = new ConcurrentDictionary<string, GlobalTool>(StringComparer.OrdinalIgnoreCase);

        for (var skip = 0; skip < max; skip += size)
        {
            var arguments = !string.IsNullOrWhiteSpace(pattern)
                ? $"tool search {parameters.Pattern} --prerelease --take {size} --skip {skip}"
                : $"tool search {_randomHelper.RandomCharacter()} --prerelease --take {size} --skip {skip}";

            await _processHelper.RunProcessAsync(name, arguments, (_, args) =>
            {
                var message = args.Data;
                var globalTool = ExtractGlobalTool(message);
                if (globalTool == null) return;
                cache.TryAdd(globalTool.Id, globalTool);
            }, cancellationToken);
        }

        var globalTools = cache.Values
            .Where(x => pattern is null || x.IsMatchingPattern(pattern))
            .OrderBy(_ => Guid.NewGuid())
            .Take(max)
            .OrderBy(x => x.Id)
            .ToList();
        return globalTools;
    }

    public async Task<ICollection<GlobalTool>> InstallGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        const string name = @"dotnet";
        var arguments = File.Exists(parameters.NugetConfigFile)
            ? $"tool install -g {{0}} --version *-* --ignore-failed-sources --configfile {parameters.NugetConfigFile}"
            : @"tool install -g {0} --version *-* --ignore-failed-sources";

        _logger.LogEmptyLineWhenLogLevelIsEnabled(LogLevel.Information);

        var ids = parameters.Ids.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        foreach (var id in ids)
        {
            _logger.LogInformation("Installing tool '{id}'", id);
            await _processHelper.RunProcessAsync(name, string.Format(arguments, id), cancellationToken);
        }

        var globalTools = await GetGlobalToolsAsync(parameters, cancellationToken);
        var installedGlobalTools = globalTools
            .Where(x => ids.Any(id => id.IgnoreCaseEquals(x.Id)))
            .OrderBy(x => x.Id)
            .ToList();

        return installedGlobalTools;
    }

    public async Task<ICollection<GlobalTool>> UninstallGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        var globalTools = await GetGlobalToolsAsync(parameters, cancellationToken);

        const string name = @"dotnet";
        const string arguments = @"tool uninstall -g {0}";

        _logger.LogEmptyLineWhenLogLevelIsEnabled(LogLevel.Information);

        var ids = parameters.Ids.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        foreach (var id in ids)
        {
            _logger.LogInformation("Uninstalling tool '{id}'", id);
            await _processHelper.RunProcessAsync(name, string.Format(arguments, id), cancellationToken);
        }

        var uninstalledGlobalTools = globalTools
            .Where(x => ids.Any(id => id.IgnoreCaseEquals(x.Id)))
            .OrderBy(x => x.Id)
            .ToList();

        return uninstalledGlobalTools;
    }

    public async Task<(ICollection<GlobalTool>, ICollection<GlobalTool>)> UpdateGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        var globalToolsBefore = await GetGlobalToolsAsync(parameters, cancellationToken);

        const string name = @"dotnet";
        var arguments = File.Exists(parameters.NugetConfigFile)
            ? $"tool update -g {{0}} --version *-* --ignore-failed-sources --configfile {parameters.NugetConfigFile}"
            : @"tool update -g {0} --version *-* --ignore-failed-sources";

        _logger.LogEmptyLineWhenLogLevelIsEnabled(LogLevel.Information);

        foreach (var globalToolBefore in globalToolsBefore.Where(x => !x.IsCurrentTool))
        {
            _logger.LogInformation("Updating tool '{name}'", globalToolBefore.Id);
            await _processHelper.RunProcessAsync(name, string.Format(arguments, globalToolBefore.Id), cancellationToken);
        }

        var globalToolsAfter = await GetGlobalToolsAsync(parameters, cancellationToken);
        return (globalToolsBefore, globalToolsAfter);
    }

    private static GlobalTool ExtractGlobalTool(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return null;
        if (message.IgnoreCaseContains("Package ID")) return null;

        const string separator = "  ";
        var parts = message
            .Split(separator)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (parts.Length == 3)
        {
            return new GlobalTool
            {
                Id = parts[0],
                Version = parts[1],
                Command = parts[2]
            };
        }

        if (parts.Length >= 4)
        {
            return new GlobalTool
            {
                Id = parts[0],
                Version = parts[1],
                Authors = parts[2],
                Downloads = parts[3]
            };
        }

        return null;
    }
}