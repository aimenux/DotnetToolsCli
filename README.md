# DotnetToolsCli
```
A net global tool to manage all other global tools
```

> In this repo, i m building a global tool that allows to list, update, install or uninstall net global tools.
>
> To install, run, update, uninstall global tool from a local source path, type commands :
> - `dotnet tool install -g --add-source .\Nugets\ --configfile .\Nugets\nuget.config DotnetToolsCli`
> - `dotnet-tools`
> - `dotnet-tools -h`
> - `dotnet-tools list`
> - `dotnet-tools update`
> - `dotnet-tools install [id]`
> - `dotnet-tools uninstall [id]`
>
> To install global tool from [nuget source](https://www.nuget.org/packages/DotnetToolsCli), type these command :
> - `dotnet tool install -g DotnetToolsCli --ignore-failed-sources`
>
>

**`Tools`** : vs22, net 6.0
