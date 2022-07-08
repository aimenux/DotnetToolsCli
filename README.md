[![.NET](https://github.com/aimenux/DotnetToolsCli/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/DotnetToolsCli/actions/workflows/ci.yml)

# DotnetToolsCli
```
Providing a net global tool to manage all other global tools
```

> In this repo, i m building a global tool that allows to manage all other global tools.
>
> The tool is based on multiple sub commmands :
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
> - `dotnet-tools Uninstall [id]` to uninstall global tools
>
> To install global tool from a local source path, type commands :
> - `dotnet tool install -g --configfile .\Nugets\local.config DotnetToolsCli --version "*-*" --ignore-failed-sources`
>
> To install global tool from [nuget source](https://www.nuget.org/packages/DotnetToolsCli), type these command :
> - For stable version : `dotnet tool install -g DotnetToolsCli --ignore-failed-sources`
> - For prerelease version : `dotnet tool install -g DotnetToolsCli --version "*-*" --ignore-failed-sources`
>
> To uninstall global tool, type these command :
> - `dotnet tool uninstall -g DotnetToolsCli`
>
>

**`Tools`** : vs22, net 6.0, command-line, spectre-console