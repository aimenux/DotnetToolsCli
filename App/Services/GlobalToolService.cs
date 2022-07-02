using System.Collections.Concurrent;
using System.Collections.Immutable;
using App.Extensions;
using App.Helpers;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace App.Services;

public class GlobalToolService : IGlobalToolService
{
    private readonly IProcessHelper _processHelper;
    private readonly ILoggingHelper _loggingHelper;
    private readonly ILogger _logger;

    public GlobalToolService(IProcessHelper processHelper, ILoggingHelper loggingHelper, ILogger logger)
    {
        _processHelper = processHelper ?? throw new ArgumentNullException(nameof(processHelper));
        _loggingHelper = loggingHelper ?? throw new ArgumentNullException(nameof(loggingHelper));
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
            .ToImmutableList();
        return globalTools;
    }

    public async Task<ICollection<GlobalTool>> InstallGlobalToolsAsync(GlobalToolsParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters.Verbose)
        {
            _loggingHelper.SetMinimumLevel(LogEventLevel.Verbose);
        }

        const string name = @"dotnet";
        var arguments = File.Exists(parameters.NugetConfigFile)
            ? $"tool install -g {{0}} --version *-* --ignore-failed-sources --configfile {parameters.NugetConfigFile}"
            : @"tool install -g {0} --version *-* --ignore-failed-sources";

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
        if (parameters.Verbose)
        {
            _loggingHelper.SetMinimumLevel(LogEventLevel.Verbose);
        }

        var globalTools = await GetGlobalToolsAsync(parameters, cancellationToken);

        const string name = @"dotnet";
        const string arguments = @"tool uninstall -g {0}";

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
        if (parameters.Verbose)
        {
            _loggingHelper.SetMinimumLevel(LogEventLevel.Verbose);
        }

        var globalToolsBefore = await GetGlobalToolsAsync(parameters, cancellationToken);

        const string name = @"dotnet";
        var arguments = File.Exists(parameters.NugetConfigFile)
            ? $"tool update -g {{0}} --version *-* --ignore-failed-sources --configfile {parameters.NugetConfigFile}"
            : @"tool update -g {0} --version *-* --ignore-failed-sources";

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

        const string separator = " ";
        var parts = message
            .Split(separator)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (parts.Length != 3) return null;

        return new GlobalTool
        {
            Id = parts[0],
            Version = parts[1],
            Command = parts[2]
        };
    }
}