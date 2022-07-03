using System.ComponentModel.DataAnnotations;
using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace App.Commands;

[Command(Name = "Uninstall", FullName = "Uninstall global tools", Description = "Uninstall global tools.")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class UninstallCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;
    private readonly ILogger _logger;

    public UninstallCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper, IFileHelper fileHelper, IGlobalToolService globalToolService, ILogger logger) : base(consoleHelper, loggingHelper, fileHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Option("-v|--verbose", "Verbose logging.", CommandOptionType.NoValue)]
    public bool Verbose { get; set; }

    [Required]
    [Argument(0, nameof(Ids), "Tool(s) Id(s) or ExportFile(s)")]
    public string[] Ids { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var ids = await FileHelper.ExtractGlobalToolsIdsAsync(Ids, cancellationToken);

        var parameters = new GlobalToolsParameters
        {
            Ids = ids
        };

        var globalTools = await _globalToolService.UninstallGlobalToolsAsync(parameters, cancellationToken);

        ConsoleHelper.RenderGlobalTools(globalTools, parameters);
    }

    protected override bool HasValidOptions()
    {
        if (Ids?.Any(x => !string.IsNullOrWhiteSpace(x)) != true)
        {
            _logger.LogError("Ids are not valid");
            return false;
        }

        return true;
    }

    protected override bool IsVerboseLoggingEnabled() => Verbose;

    private static string GetVersion() => GetVersion(typeof(UninstallCommand));
}