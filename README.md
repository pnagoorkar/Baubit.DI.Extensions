# Baubit.DI.Extensions

[![CircleCI](https://dl.circleci.com/status-badge/img/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master)
[![codecov](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions/branch/master/graph/badge.svg)](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions)<br/>
[![NuGet](https://img.shields.io/nuget/v/Baubit.DI.Extensions.svg)](https://www.nuget.org/packages/Baubit.DI.Extensions/)
[![NuGet](https://img.shields.io/nuget/dt/Baubit.DI.Extensions.svg)](https://www.nuget.org/packages/Baubit.DI.Extensions) <br/>
![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4?logo=dotnet&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)<br/>
[![Known Vulnerabilities](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions/badge.svg)](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions)

Extensions for [Baubit.DI](https://github.com/pnagoorkar/Baubit.DI).

## Installation

```bash
dotnet add package Baubit.DI.Extensions
```

## Usage

### Adding Modules Directly to IServiceCollection

```csharp
// Using configuration action with fluent chaining
var provider = new ServiceCollection()
    .AddModule<MyModule, MyConfiguration>(cfg => cfg.ConnectionString = "Server=localhost")
    .AddSingleton<ILogger, ConsoleLogger>()
    .BuildServiceProvider();

// Using configuration builder action
var provider = new ServiceCollection()
    .AddModule<MyModule, MyConfiguration>(builder => 
        builder.WithRawJsonStrings("{\"ConnectionString\": \"Server=localhost\"}"))
    .BuildServiceProvider();
```

### Building Service Provider from ComponentBuilder

```csharp
var result = ComponentBuilder.CreateNew()
    .WithModule<MyModule, MyConfiguration>(cfg => cfg.ConnectionString = "Server=localhost")
    .BuildServiceProvider();

if (result.IsSuccess)
{
    IServiceProvider serviceProvider = result.Value;
}
```

## API Reference

| Method | Description |
|--------|-------------|
| `AddModule<TModule, TConfiguration>(this IServiceCollection, Action<TConfiguration>)` | Adds a module to the service collection using configuration action |
| `AddModule<TModule, TConfiguration>(this IServiceCollection, Action<ConfigurationBuilder<TConfiguration>>)` | Adds a module to the service collection using configuration builder action |
| `BuildServiceProvider(this IComponent, IServiceCollection?)` | Builds a service provider from a component |
| `BuildServiceProvider(this Result<IComponent>, IServiceCollection?)` | Builds a service provider from a component result |
| `BuildServiceProvider(this Result<ComponentBuilder>, IServiceCollection?)` | Builds component and creates a service provider |

## License

MIT
