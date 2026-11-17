# Loom.Net

[![Publish.Nuget](https://github.com/joshua-nixon/Loom.Net/actions/workflows/Publish.Nuget.yaml/badge.svg)](https://github.com/joshua-nixon/Loom.Net/actions/workflows/Publish.Nuget.yaml)
![NuGet Version](https://img.shields.io/nuget/vpre/Loom.Net.AspNetCore)
![NuGet Downloads](https://img.shields.io/nuget/dt/Loom.Net.AspNetCore?color=blue)

Loom.Net is an early-stage reverse proxy for ASP.NET Core. It routes requests by path template and forwards them to configured backends.

## Install

```shell
dotnet add package Loom.Net.AspNetCore
```

Loom.Net targets .NET 11 and uses the ASP.NET Core shared framework.

## Configure routes

Add a `Loom` section to `appsettings.json`. Each route references a named backend and defines the path to match and the upstream path to request.

```json
{
  "Loom": {
    "Backends": {
      "users-service": {
        "Host": "localhost",
        "Port": 5237
      }
    },
    "Routes": [
      {
        "PathTemplate": "user/{user-id}",
        "Upstream": {
          "Name": "users-service",
          "PathTemplate": "user?id={user-id}"
        }
      }
    ]
  }
}
```

With this configuration, `GET /loom/user/42` is routed to `http://localhost:5237/user?id=42`. Path parameters match one non-empty path segment. If both the upstream template and incoming request provide the same query key, the value from the upstream template takes precedence.

## Register Loom

Load the `Loom` section and map Loom's catch-all endpoint:

```csharp
using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLoom(options =>
    options.LoadFromConfiguration(builder.Configuration)
);

var app = builder.Build();

app.UseLoom();
app.Run();
```

Requests under the configured route prefix are handled by Loom. Requests that do not match a configured route receive a `404`.

## Development

Loom.Net is under active development and is not yet a general-purpose reverse proxy.