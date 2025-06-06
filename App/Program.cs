﻿using App.Commands;
using App.Extensions;
using App.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await CreateHostBuilder(args).RunCommandLineApplicationAsync<MainCommand>(args);
        }
        catch (Exception ex)
        {
            ConsoleHelper.RenderAnyException(ex);
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile();
                config.AddUserSecrets();
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddDefaultLogger();
                loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.Scan(scan =>
                {
                    scan.FromAssemblies(typeof(Program).Assembly)
                        .AddClasses(classes => classes.InNamespaceOf<Settings>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
                });

                services.Configure<Settings>(hostingContext.Configuration.GetSection(nameof(Settings)));
            })
            .AddSerilog();
}