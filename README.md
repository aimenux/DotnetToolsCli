[![.NET](https://github.com/aimenux/DotnetToolsCli/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/DotnetToolsCli/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/DotnetToolsCli)](https://www.nuget.org/packages/DotnetToolsCli/)

# DotnetToolsCli
```
Providing a dotnet cli tool to manage dotnet global tools
```

> In this repo, i m building a cli tool that allows to manage dotnet global tools.
>
> The tool is based on multiple sub commands :
> - Use sub command `List` to list installed global tools
> - Use sub command `Search` to search global tools published on [nuget](https://www.nuget.org/packages?packagetype=dotnettool)
> - Use sub command `Update` to update installed global tools
> - Use sub command `Install` to install global tools
> - Use sub command `Uninstall` to uninstall global tools
>
> To run the tool, type commands :
> - `dotnet-tools -h` to show help
> - `dotnet-tools -s` to show settings
> - `dotnet-tools List` to list installed global tools
> - `dotnet-tools List -p [pattern]` to list installed global tools matching pattern
> - `dotnet-tools Search` to search public global tools
> - `dotnet-tools Search -p [pattern]` to search public global tools matching pattern
> - `dotnet-tools Update` to update installed global tools
> - `dotnet-tools Update -p [pattern]` to update installed global tools matching pattern
> - `dotnet-tools Install [id]` to install global tools
> - `dotnet-tools Install [id] --force` to force install global tools
> - `dotnet-tools Install [id] --version [version]` to install global tools with some version
> - `dotnet-tools Uninstall [id]` to uninstall global tools
>
> To install cli tool from a local source path, type commands :
> - `dotnet tool install -g --configfile .\Nugets\local.config DotnetToolsCli --version "*-*" --ignore-failed-sources`
>
> To install cli tool from [nuget source](https://www.nuget.org/packages/DotnetToolsCli), type this command :
> - For stable version : `dotnet tool install -g DotnetToolsCli --ignore-failed-sources`
> - For prerelease version : `dotnet tool install -g DotnetToolsCli --version "*-*" --ignore-failed-sources`
>
> To uninstall cli tool, type this command :
> - `dotnet tool uninstall -g DotnetToolsCli`
>
>

**`Tools`** : net 9.0, command-line, spectre-console