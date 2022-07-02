using App.Helpers;
using McMaster.Extensions.CommandLineUtils;
using static App.Extensions.PathExtensions;

namespace App.Commands;

[Command(Name = Settings.CommandName, FullName = "Global tools cli", Description = "A net global tool helping to manage all net global tools.")]
[Subcommand(typeof(ListCommand), typeof(SearchCommand), typeof(UpdateCommand), typeof(InstallCommand), typeof(UninstallCommand))]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
public class MainCommand : AbstractCommand
{
    public MainCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper) : base(consoleHelper, loggingHelper)
    {
    }

    [Option("-s|--settings", "Show settings information.", CommandOptionType.NoValue)]
    public bool ShowSettings { get; set; }

    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        if (ShowSettings)
        {
            var filepath = GetSettingFilePath();
            ConsoleHelper.RenderSettingsFile(filepath);
        }
        else
        {
            ConsoleHelper.RenderTitle(Settings.CommandName);
            app.ShowHelp();
        }

        return Task.CompletedTask;
    }

    private static string GetVersion() => GetVersion(typeof(MainCommand));
}