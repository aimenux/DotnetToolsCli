using App.Services;

namespace App.Helpers;

public interface IConsoleHelper
{
    void RenderTitle(string text);

    Task RenderStatusAsync(Func<Task> action);

    void RenderSettingsFile(string filepath);

    void RenderException(Exception exception);

    void RenderGlobalTools(ICollection<GlobalTool> globalTools, GlobalToolsParameters parameters);

    void RenderGlobalTools(ICollection<GlobalTool> globalToolsBefore, ICollection<GlobalTool> globalToolsAfter, GlobalToolsParameters parameters);
}