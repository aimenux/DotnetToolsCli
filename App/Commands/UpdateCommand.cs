using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace App.Commands;

[Command(Name = "Update", FullName = "Update global tools", Description = "Update global tools.")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class UpdateCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;
    private readonly ILogger _logger;

    public UpdateCommand(IConsoleHelper consoleHelper, IGlobalToolService globalToolService, ILogger logger) : base(consoleHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Option("-v|--verbose", "Verbose logging.", CommandOptionType.NoValue)]
    public bool Verbose { get; set; }

    [Option("-p|--pattern", "Pattern matching.", CommandOptionType.SingleValue)]
    public string Pattern { get; set; }

    [Option("-f|--file", "Nuget configuration file to use.", CommandOptionType.SingleValue)]
    public string NugetConfigFile { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new GlobalToolsParameters
        {
            Verbose = Verbose,
            Pattern = Pattern,
            NugetConfigFile = NugetConfigFile
        };
        var (globalToolsBefore, globalToolsAfter) = await _globalToolService.UpdateGlobalToolsAsync(parameters, cancellationToken);
        ConsoleHelper.RenderGlobalTools(globalToolsBefore, globalToolsAfter, parameters);
    }

    protected override bool HasValidOptions()
    {
        if (Pattern != null && string.IsNullOrWhiteSpace(Pattern))
        {
            _logger.LogError("Pattern '{pattern}' is not valid", Pattern);
            return false;
        }

        if (!string.IsNullOrEmpty(NugetConfigFile) && File.Exists(NugetConfigFile))
        {
            _logger.LogError("Nuget file '{name}' does not exist", NugetConfigFile);
            return false;
        }

        return true;
    }

    private static string GetVersion() => GetVersion(typeof(UpdateCommand));
}