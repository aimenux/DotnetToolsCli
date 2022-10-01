using System.ComponentModel.DataAnnotations;
using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace App.Commands;

[Command(Name = "Search", FullName = "Search global tools", Description = "Search global tools.")]
public class SearchCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;
    private readonly ILogger _logger;

    public SearchCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper, IFileHelper fileHelper, IGlobalToolService globalToolService, ILogger logger) : base(consoleHelper, loggingHelper, fileHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Range(1, 1000)]
    [Option("-m|--max", "MaxItems.", CommandOptionType.SingleValue)]
    public int MaxItems { get; set; } = 30;

    [Option("-p|--pattern", "Pattern matching.", CommandOptionType.SingleValue)]
    public string Pattern { get; set; }

    [Option("-x|--export", "ExportDirectory", CommandOptionType.SingleValue)]
    public string ExportDirectory { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new GlobalToolsParameters
        {
            Pattern = Pattern,
            MaxItems = MaxItems,
            ExportDirectory = ExportDirectory
        };

        await ConsoleHelper.RenderStatusAsync(async () =>
        {
            var globalTools = await _globalToolService.SearchGlobalToolsAsync(parameters, cancellationToken);

            await FileHelper.ExportGlobalToolsAsync(globalTools, parameters, cancellationToken);

            ConsoleHelper.RenderGlobalTools(globalTools, parameters);
        });
    }

    protected override bool HasValidOptions()
    {
        if (string.IsNullOrWhiteSpace(ExportDirectory)) return true;
        if (Directory.Exists(ExportDirectory)) return true;
        _logger.LogError($"Export directory '{ExportDirectory}' does not exist");
        return false;
    }
}