# Baubit.DI.Extensions

[![CircleCI](https://dl.circleci.com/status-badge/img/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master)
[![codecov](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions/branch/master/graph/badge.svg)](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions)<br/>
[![NuGet](https://img.shields.io/nuget/v/Baubit.DI.Extensions.svg)](https://www.nuget.org/packages/Baubit.DI.Extensions/)
![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4?logo=dotnet&logoColor=white)<br/>
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Known Vulnerabilities](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions/badge.svg)](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions)

Extensions for [Baubit.DI](https://github.com/pnagoorkar/Baubit.DI) modularity framework.

## Installation

```bash
dotnet add package Baubit.DI.Extensions
```

## Overview

This package provides `ComponentBuilder<T>`, a generic builder that creates a fully configured service from a collection of modules. It extends `ComponentBuilder` from Baubit.DI to resolve a specific service type after all modules have loaded their registrations.

## Quick Start

```csharp
// Define configuration and module
public class MyConfiguration : AConfiguration
{
    public string ConnectionString { get; set; }
}

public class MyModule : AModule<MyConfiguration>
{
    public MyModule(MyConfiguration configuration, List<IModule>? nestedModules = null)
        : base(configuration, nestedModules) { }

    public override void Load(IServiceCollection services)
    {
        services.AddSingleton<IMyService>(new MyService(Configuration.ConnectionString));
        base.Load(services);
    }
}

// Build and resolve service
var builder = new ComponentBuilder<IMyService>();
builder.WithModule<MyModule, MyConfiguration>(cfg =>
{
    cfg.ConnectionString = "Server=localhost;Database=mydb";
});

var result = builder.Build();
if (result.IsSuccess)
{
    IMyService service = result.Value;
}
```

## API Reference

<details>
<summary><strong>ComponentBuilder&lt;T&gt;</strong></summary>

Generic builder for creating a configured service of type `T` from modules.

| Method | Description |
|--------|-------------|
| `Build()` | Builds the component and resolves a service of type `T` |

Inherits from `ComponentBuilder`:
| Method | Description |
|--------|-------------|
| `WithModule<TModule, TConfiguration>(Action<TConfiguration>)` | Add a module with configuration |
| `WithModulesFrom(params IComponent[])` | Add modules from existing components |

</details>

## License

MIT
