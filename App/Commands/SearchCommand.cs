using System.ComponentModel.DataAnnotations;
using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command(Name = "Search", FullName = "Search global tools", Description = "Search global tools.")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class SearchCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;

    public SearchCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper, IGlobalToolService globalToolService) : base(consoleHelper, loggingHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
    }

    [Range(1, 100)]
    [Option("-m|--max", "MaxItems.", CommandOptionType.SingleValue)]
    public int MaxItems { get; set; } = 30;

    [Option("-p|--pattern", "Pattern matching.", CommandOptionType.SingleValue)]
    public string Pattern { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new GlobalToolsParameters
        {
            Pattern = Pattern,
            MaxItems = MaxItems
        };

        await ConsoleHelper.RenderStatusAsync(async () =>
        {
            var globalTools = await _globalToolService.SearchGlobalToolsAsync(parameters, cancellationToken);

            ConsoleHelper.RenderGlobalTools(globalTools, parameters);
        });
    }

    private static string GetVersion() => GetVersion(typeof(SearchCommand));
}