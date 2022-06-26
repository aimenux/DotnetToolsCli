using App.Helpers;
using App.Services;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

[Command(Name = "List", FullName = "List global tools", Description = "List global tools.")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class ListCommand : AbstractCommand
{
    private readonly IGlobalToolService _globalToolService;

    public ListCommand(IConsoleHelper consoleHelper, IGlobalToolService globalToolService) : base(consoleHelper)
    {
        _globalToolService = globalToolService ?? throw new ArgumentNullException(nameof(globalToolService));
    }

    [Option("-p|--pattern", "Pattern matching.", CommandOptionType.SingleValue)]
    public string Pattern { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        var parameters = new GlobalToolsParameters
        {
            Pattern = Pattern
        };
        var globalTools = await _globalToolService.GetGlobalToolsAsync(parameters, cancellationToken);
        ConsoleHelper.RenderGlobalTools(globalTools, parameters);
    }

    private static string GetVersion() => GetVersion(typeof(ListCommand));
}