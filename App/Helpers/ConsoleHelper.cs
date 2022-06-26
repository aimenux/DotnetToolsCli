using System.Text;
using App.Extensions;
using App.Services;
using Spectre.Console;

namespace App.Helpers;

public class ConsoleHelper : IConsoleHelper
{
    private static readonly IRandomHelper RandomHelper = new RandomHelper();

    public ConsoleHelper()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    public void RenderTitle(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText(text).LeftAligned());
        AnsiConsole.WriteLine();
    }

    public async Task RenderStatusAsync(Func<Task> action)
    {
        var spinner = RandomHelper.RandomSpinner();

        await AnsiConsole.Status()
            .StartAsync("Work is in progress ...", async ctx =>
            {
                ctx.Spinner(spinner);
                await action.Invoke();
            });
    }

    public void RenderSettingsFile(string filepath)
    {
        var name = Path.GetFileName(filepath);
        var json = File.ReadAllText(filepath);
        var formattedJson = json.GetFormattedJson();
        var header = new Rule($"[yellow]({name})[/]");
        header.Centered();
        var footer = new Rule($"[yellow]({filepath})[/]");
        footer.Centered();

        AnsiConsole.WriteLine();
        AnsiConsole.Write(header);
        AnsiConsole.WriteLine(formattedJson);
        AnsiConsole.Write(footer);
        AnsiConsole.WriteLine();
    }

    public void RenderException(Exception exception)
    {
        const ExceptionFormats formats = ExceptionFormats.ShortenTypes
                                         | ExceptionFormats.ShortenPaths
                                         | ExceptionFormats.ShortenMethods;

        AnsiConsole.WriteLine();
        AnsiConsole.WriteException(exception, formats);
        AnsiConsole.WriteLine();
    }

    public void RenderGlobalTools(ICollection<GlobalTool> globalTools, GlobalToolsParameters parameters)
    {
        var pattern = parameters.Pattern;
        var filteredGlobalTools = globalTools
            .Where(x => pattern is null || x.IsMatchingPattern(pattern))
            .ToList();

        var count = filteredGlobalTools.Count;

        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.White)
            .Title($"[yellow]Found {count} global tool(s)[/]")
            .AddColumn(new TableColumn("[u]Id[/]").Centered())
            .AddColumn(new TableColumn("[u]CommandName[/]").Centered())
            .AddColumn(new TableColumn("[u]CurrentVersion[/]").Centered());

        if (pattern != null)
        {
            table.Caption($"Pattern is '{pattern}'");
        }

        foreach (var globalTool in filteredGlobalTools)
        {
            table.AddRow(globalTool.Id.ToMarkup(), globalTool.Command.ToMarkup(), globalTool.Version.ToMarkup());
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderGlobalTools(ICollection<GlobalTool> globalToolsBefore, ICollection<GlobalTool> globalToolsAfter, GlobalToolsParameters parameters)
    {
        var pattern = parameters.Pattern;
        var filteredGlobalTools = globalToolsBefore
            .Where(x => pattern is null || x.IsMatchingPattern(pattern))
            .ToList();

        var count = filteredGlobalTools.Count;

        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.White)
            .Title($"[yellow]Found {count} global tool(s)[/]")
            .AddColumn(new TableColumn("[u]Id[/]").Centered())
            .AddColumn(new TableColumn("[u]CommandName[/]").Centered())
            .AddColumn(new TableColumn("[u]PreviousVersion[/]").Centered())
            .AddColumn(new TableColumn("[u]CurrentVersion[/]").Centered());

        if (pattern != null)
        {
            table.Caption($"Pattern is '{pattern}'");
        }

        foreach (var globalToolBefore in filteredGlobalTools)
        {
            var currentVersion = GetCurrentVersion(globalToolsAfter, globalToolBefore);
            table.AddRow(globalToolBefore.Id.ToMarkup(), globalToolBefore.Command.ToMarkup(), globalToolBefore.Version.ToMarkup(), currentVersion.ToMarkup());
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string GetCurrentVersion(IEnumerable<GlobalTool> globalToolsAfter, GlobalTool before)
    {
        var previousVersion = before.Version;
        var after = globalToolsAfter.FirstOrDefault(x => x.Id == before.Id);
        var currentVersion = after?.Version ?? previousVersion;
        return currentVersion != previousVersion ? $"[green]{currentVersion}[/]" : currentVersion;
    }
}