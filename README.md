# DotnetToolsCli
```
A net global tool to manage all other global tools
```

> In this repo, i m building a global tool that allows to list, update, install or uninstall net global tools.
>
> To uninstall, install, run, update, uninstall global tool from a local source path, type commands :
> - `dotnet tool uninstall -g DotnetToolsCli`
> - `dotnet tool install -g DotnetToolsCli --version *-* --ignore-failed-sources --configfile .\Nugets\local.config`
> - `dotnet-tools`
> - `dotnet-tools -h`
> - `dotnet-tools list`
> - `dotnet-tools update`
> - `dotnet-tools install [id]`
> - `dotnet-tools uninstall [id]`
>
> To install global tool from [nuget source](https://www.nuget.org/packages/DotnetToolsCli), type these command :
> - for beta versions : `dotnet tool install -g DotnetToolsCli --version *-* --ignore-failed-sources`
> - for stable versions : `dotnet tool install -g DotnetToolsCli --ignore-failed-sources`
>
>

**`Tools`** : vs22, net 6.0
