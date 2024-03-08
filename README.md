![Tests](https://github.com/BitzArt/Blazor.Cookies/actions/workflows/Tests.yml/badge.svg)

[![NuGet version](https://img.shields.io/nuget/v/BitzArt.Blazor.Cookies.svg)](https://www.nuget.org/packages/BitzArt.Blazor.Cookies/)
[![NuGet downloads](https://img.shields.io/nuget/dt/BitzArt.Blazor.Cookies.svg)](https://www.nuget.org/packages/BitzArt.Blazor.Cookies/)

## Overview

**BitzArt.Blazor.Cookies** is a nuget package that simplifies working with browser cookies in Blazor applications.

You can use cookies in your Blazor applications as a way to store user's unique information, such as preferences, settings, or session / authentication data.

- Built for dotnet 8+
- Supports all Blazor United render modes
- Supports Blazor prerendering

| Blazor Rendermode       | Support |
|-------------------------|:-------:|
| Static SSR              | ✔️     |
| Interactive Server      | ✔️     |
| Interactive WebAssembly | ✔️     |
| Interactive Auto        | ✔️     |

### Installation

- Install the following package in your Blazor Server project:

```
dotnet add package BitzArt.Blazor.Cookies.Server
```

- Add this line to your Server project `program.cs`:

```csharp
builder.AddBlazorCookies();
```

- Install the following package in your Blazor Client project:

```
dotnet add package BitzArt.Blazor.Cookies.Client
```

- Add this line to your Client project `program.cs`:

```csharp
builder.AddBlazorCookies();
```

### Usage

- Inject `ICookieService` in any of your Services / Blazor Components
- Use `ICookieService` to interact with user's cookies.

## License

[![License](https://img.shields.io/badge/mit-%230072C6?style=for-the-badge)](https://github.com/BitzArt/Blazor.Cookies/blob/main/LICENSE)
