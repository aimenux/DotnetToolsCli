using App.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace App.Helpers;

public class LoggingHelper : ILoggingHelper
{
    private readonly IConfiguration _configuration;

    public LoggingHelper(IConfiguration configuration)
    {
        LevelSwitch = new LoggingLevelSwitch();
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public LoggingLevelSwitch LevelSwitch { get; }

    public void SetMinimumLevelFromConfiguration()
    {
        var minLogLevel = _configuration.GetDefaultLogLevel();
        SetMinimumLevel(minLogLevel);
    }

    public void SetMinimumLevel(LogEventLevel level)
    {
        LevelSwitch.MinimumLevel = level;
    }
}