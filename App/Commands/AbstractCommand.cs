using System.Reflection;
using App.Helpers;
using McMaster.Extensions.CommandLineUtils;
using Serilog.Events;

namespace App.Commands;

public abstract class AbstractCommand
{
    protected IConsoleHelper ConsoleHelper;
    private readonly ILoggingHelper _loggingHelper;

    protected string CommandName => GetType().Name;

    protected AbstractCommand(IConsoleHelper consoleHelper, ILoggingHelper loggingHelper)
    {
        ConsoleHelper = consoleHelper ?? throw new ArgumentNullException(nameof(consoleHelper));
        _loggingHelper = loggingHelper ?? throw new ArgumentNullException(nameof(loggingHelper));
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

            if (IsVerboseLoggingEnabled())
            {
                _loggingHelper.SetMinimumLevel(LogEventLevel.Verbose);
            }

            await ExecuteAsync(app, cancellationToken);

            if (IsVerboseLoggingEnabled())
            {
                _loggingHelper.SetMinimumLevelFromConfiguration();
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.RenderException(ex);
        }
    }

    protected abstract Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default);

    protected virtual bool HasValidOptions() => true;

    protected virtual bool HasValidArguments() => true;

    protected virtual bool IsVerboseLoggingEnabled() => false;

    protected static string GetVersion(Type type)
    {
        return type
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }
}