using System.Text;
using App.Extensions;
using App.Services;
using Spectre.Console;

namespace App.Helpers;

public sealed class ConsoleHelper : IConsoleHelper
{
    private readonly IRandomHelper _randomHelper;

    private ConsoleHelper()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    public ConsoleHelper(IRandomHelper randomHelper) : this()
    {
        _randomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));
    }

    public void RenderTitle(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText(text));
        AnsiConsole.WriteLine();
    }

    public async Task RenderStatusAsync(Func<Task> action)
    {
        var spinner = _randomHelper.RandomSpinner();

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

    public void RenderException(Exception exception) => RenderAnyException(exception);

    public static void RenderAnyException<T>(T exception) where T : Exception
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
        var count = globalTools.Count;
        var caption = BuildCustomCaption(parameters);

        var isSearch = parameters.MaxItems > 0 || globalTools.Any(x => !string.IsNullOrWhiteSpace(x.Downloads));
        if (isSearch)
        {
            var table = new Table()
                .Border(TableBorder.Square)
                .BorderColor(Color.White)
                .Title($"[yellow]Found {count} global tool(s)[/]")
                .AddColumn(new TableColumn("[u]Id[/]").Centered())
                .AddColumn(new TableColumn("[u]LatestVersion[/]").Centered())
                .AddColumn(new TableColumn("[u]Authors[/]").Centered())
                .AddColumn(new TableColumn("[u]Downloads[/]").Centered());

            if (!string.IsNullOrWhiteSpace(caption))
            {
                table.Caption(caption);
            }
            else
            {
                if (count >= 100)
                {
                    table.Caption($"[grey]Found {count} global tool(s)[/]");
                }
            }

            foreach (var globalTool in globalTools)
            {
                table.AddRow(globalTool.Id.ToMarkup(), globalTool.Version.ToMarkup(), globalTool.Authors.ToMarkup(), globalTool.Downloads.ToMarkup());
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
        else
        {
            var table = new Table()
                .Border(TableBorder.Square)
                .BorderColor(Color.White)
                .Title($"[yellow]Found {count} global tool(s)[/]")
                .AddColumn(new TableColumn("[u]Id[/]").Centered())
                .AddColumn(new TableColumn("[u]CommandName[/]").Centered())
                .AddColumn(new TableColumn("[u]CurrentVersion[/]").Centered());

            if (!string.IsNullOrWhiteSpace(caption))
            {
                table.Caption(caption);
            }
            else
            {
                if (count >= 100)
                {
                    table.Caption($"[grey]Found {count} global tool(s)[/]");
                }
            }

            foreach (var globalTool in globalTools)
            {
                table.AddRow(globalTool.Id.ToMarkup(), globalTool.Command.ToMarkup(), globalTool.Version.ToMarkup());
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }

    public void RenderGlobalTools(ICollection<GlobalTool> globalToolsBefore, ICollection<GlobalTool> globalToolsAfter, GlobalToolsParameters parameters)
    {
        var count = globalToolsBefore.Count;

        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.White)
            .Title($"[yellow]Found {count} global tool(s)[/]")
            .AddColumn(new TableColumn("[u]Id[/]").Centered())
            .AddColumn(new TableColumn("[u]CommandName[/]").Centered())
            .AddColumn(new TableColumn("[u]PreviousVersion[/]").Centered())
            .AddColumn(new TableColumn("[u]CurrentVersion[/]").Centered());

        var pattern = parameters.Pattern;
        if (pattern != null)
        {
            table.Caption($"Pattern is '{pattern}'");
        }

        foreach (var globalToolBefore in globalToolsBefore)
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

    private static string BuildCustomCaption(GlobalToolsParameters parameters)
    {
        var pattern = parameters.Pattern;
        var exportFile = parameters.ExportFile;
        var captionBuilder = new StringBuilder();

        if (pattern != null)
        {
            captionBuilder.Append($"Pattern is '{pattern}'");
            if (exportFile != null)
            {
                captionBuilder.AppendLine();
            }
        }
        if (exportFile != null)
        {
            captionBuilder.Append($"ExportFile is '[u][b][grey][link={exportFile}]{exportFile}[/][/][/][/]'");
        }

        var caption = captionBuilder.ToString();
        return caption;
    }
}