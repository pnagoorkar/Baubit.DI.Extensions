# Baubit.DI.Extensions

[![CircleCI](https://dl.circleci.com/status-badge/img/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/circleci/TpM4QUH8Djox7cjDaNpup5/2zTgJzKbD2m3nXCf5LKvqS/tree/master)
[![codecov](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions/branch/master/graph/badge.svg)](https://codecov.io/gh/pnagoorkar/Baubit.DI.Extensions)<br/>
[![NuGet](https://img.shields.io/nuget/v/Baubit.DI.Extensions.svg)](https://www.nuget.org/packages/Baubit.DI.Extensions/)
![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4?logo=dotnet&logoColor=white)<br/>
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Known Vulnerabilities](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions/badge.svg)](https://snyk.io/test/github/pnagoorkar/Baubit.DI.Extensions)

Extensions for [Baubit.DI](https://github.com/pnagoorkar/Baubit.DI).

## Installation

```bash
dotnet add package Baubit.DI.Extensions
```

## Usage

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
| `BuildServiceProvider(this IComponent)` | Builds a service provider from a component |
| `BuildServiceProvider(this Result<IComponent>)` | Builds a service provider from a component result |
| `BuildServiceProvider(this Result<ComponentBuilder>)` | Builds component and creates a service provider |

## License

MIT
