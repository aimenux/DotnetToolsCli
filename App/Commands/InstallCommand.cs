using System.ComponentModel.DataAnnotations;
using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace App.Commands;

[Command(Name = "Install", FullName = "Install global tools", Description = "Install global tools.")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class InstallCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;
    private readonly ILogger _logger;

    public InstallCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper, IGlobalToolService globalToolService, ILogger logger) : base(consoleHelper, loggingHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Option("-v|--verbose", "Verbose logging.", CommandOptionType.NoValue)]
    public bool Verbose { get; set; }

    [Option("-f|--file", "Nuget configuration file to use.", CommandOptionType.SingleValue)]
    public string NugetConfigFile { get; set; }

    [Required]
    [Argument(0, nameof(Ids))]
    public string[] Ids { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new GlobalToolsParameters
        {
            Ids = Ids,
            NugetConfigFile = NugetConfigFile
        };

        var globalTools = await _globalToolService.InstallGlobalToolsAsync(parameters, cancellationToken);

        ConsoleHelper.RenderGlobalTools(globalTools, parameters);
    }

    protected override bool HasValidOptions()
    {
        if (!string.IsNullOrEmpty(NugetConfigFile) && File.Exists(NugetConfigFile))
        {
            _logger.LogError("Nuget file '{name}' does not exist", NugetConfigFile);
            return false;
        }

        if (Ids?.Any(x => !string.IsNullOrWhiteSpace(x)) != true)
        {
            _logger.LogError("Ids are not valid");
            return false;
        }

        return true;
    }

    protected override bool IsVerboseLoggingEnabled() => Verbose;

    private static string GetVersion() => GetVersion(typeof(InstallCommand));
}