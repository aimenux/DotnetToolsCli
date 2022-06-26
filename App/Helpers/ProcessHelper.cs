using System.Diagnostics;
using App.Extensions;
using Microsoft.Extensions.Logging;

namespace App.Helpers;

public class ProcessHelper : IProcessHelper
{
    private readonly ILogger _logger;

    public ProcessHelper(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RunProcessAsync(string name, string arguments, CancellationToken cancellationToken)
    {
        void OutputDataReceived(object _, DataReceivedEventArgs args) => LogProcessMessage(args.Data);
        void ErrorDataReceived(object _, DataReceivedEventArgs args) => LogProcessMessage(args.Data);
        await RunProcessAsync(name, arguments, OutputDataReceived, ErrorDataReceived, cancellationToken);
    }

    public async Task RunProcessAsync(string name, string arguments, DataReceivedEventHandler outputDataReceived, CancellationToken cancellationToken)
    {
        void ErrorDataReceived(object _, DataReceivedEventArgs args) => LogProcessMessage(args.Data);
        await RunProcessAsync(name, arguments, outputDataReceived, ErrorDataReceived, cancellationToken);
    }

    public async Task RunProcessAsync(
        string name,
        string arguments,
        DataReceivedEventHandler outputDataReceived,
        DataReceivedEventHandler errorDataReceived,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = $@"{name}",
            Arguments = $@"{arguments}"
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        process.ErrorDataReceived += errorDataReceived;
        process.OutputDataReceived += outputDataReceived;

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync(cancellationToken);
        process.Close();
    }

    private void LogProcessMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        if (message.IgnoreCaseContains("failure") && message.IgnoreCaseContains("caused by"))
        {
            return;
        }

        if (message.IgnoreCaseContains("https://aka.ms/failure-installing-tool"))
        {
            return;
        }

        if (message.IgnoreCaseContains("tool") && message.IgnoreCaseContains("fail"))
        {
            const string pattern = "due to the following:";
            if (message.EndsWith(pattern))
            {
                message = message.Replace(pattern, string.Empty);
            }

            _logger.LogError(message);
            return;
        }

        if (message.IgnoreCaseContains("tool") && message.IgnoreCaseContains("install") && message.IgnoreCaseContains("version"))
        {
            _logger.LogInformation(message);
            return;
        }

        if (message.IgnoreCaseContains("tool") && message.IgnoreCaseContains("package id") && message.IgnoreCaseContains("not be found"))
        {
            _logger.LogWarning(message);
            return;
        }

        if (message.IgnoreCaseContains("tool") && message.IgnoreCaseContains("already installed"))
        {
            _logger.LogWarning(message);
            return;
        }

        var messagesToIgnore = new[]
        {
            "dotnet tool list",
            "Tools are uninstalled using their package Id",
            "from the tool name you use when calling the tool",
            "the corresponding package Ids for installed tools"
        };

        if (messagesToIgnore.Any(x => message.IgnoreCaseContains(x)))
        {
            return;
        }

        _logger.LogTrace(message);
    }
}