using App.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Spectre.Console;
using static App.Extensions.PathExtensions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace App.Extensions;

public static class LoggingExtensions
{
    public static void AddDefaultLogger(this ILoggingBuilder loggingBuilder)
    {
        const string categoryName = Settings.PackageId;
        var services = loggingBuilder.Services;
        services.AddSingleton(serviceProvider =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(categoryName);
        });
    }

    public static IHostBuilder AddSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((hostingContext, serviceProvider, loggerConfiguration) =>
        {
            SelfLog.Enable(Console.Error);

            var loggingHelper = serviceProvider.GetRequiredService<ILoggingHelper>();
            loggingHelper.SetMinimumLevelFromConfiguration();

            var settingsFile = GetSettingFilePath();
            if (File.Exists(settingsFile))
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .MinimumLevel.ControlledBy(loggingHelper.LevelSwitch);
            }
            else
            {
                var outputTemplate = hostingContext.Configuration.GetOutputTemplate();
                loggerConfiguration
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .MinimumLevel.ControlledBy(loggingHelper.LevelSwitch);
            }
        });
    }

    public static void LogEmptyLineWhenLogLevelIsEnabled(this ILogger logger, LogLevel logeLevel)
    {
        if (logger.IsEnabled(logeLevel))
        {
            AnsiConsole.WriteLine();
        }
    }
}