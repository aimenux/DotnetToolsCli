using System.Reflection;
using App.Helpers;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands;

public abstract class AbstractCommand
{
    protected IConsoleHelper ConsoleHelper;

    protected string CommandName => GetType().Name;

    protected AbstractCommand(IConsoleHelper consoleHelper)
    {
        ConsoleHelper = consoleHelper ?? throw new ArgumentNullException(nameof(consoleHelper));
    }

    public async Task OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!HasValidOptions())
            {
                throw new Exception($"Invalid options for command {CommandName}");
            }

            if (!HasValidArguments())
            {
                throw new Exception($"Invalid arguments for command {CommandName}");
            }

            await ExecuteAsync(app, cancellationToken);
        }
        catch (Exception ex)
        {
            ConsoleHelper.RenderException(ex);
        }
    }

    protected abstract Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default);

    protected virtual bool HasValidOptions() => true;

    protected virtual bool HasValidArguments() => true;

    protected static string GetVersion(Type type)
    {
        return type
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }
}