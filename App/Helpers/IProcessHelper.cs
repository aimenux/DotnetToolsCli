using System.Diagnostics;

namespace App.Helpers;

public interface IProcessHelper
{
    Task RunProcessAsync(string name, string arguments, CancellationToken cancellationToken = default);
    Task RunProcessAsync(string name, string arguments, DataReceivedEventHandler outputDataReceived, CancellationToken cancellationToken = default);
    Task RunProcessAsync(string name, string arguments, DataReceivedEventHandler outputDataReceived, DataReceivedEventHandler errorDataReceived, CancellationToken cancellationToken = default);
}