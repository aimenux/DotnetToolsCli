using Serilog.Core;
using Serilog.Events;

namespace App.Helpers;

public interface ILoggingHelper
{
    public LoggingLevelSwitch LevelSwitch { get; }

    void SetMinimumLevelFromConfiguration();

    void SetMinimumLevel(LogEventLevel level);
}