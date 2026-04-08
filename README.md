# Verify Issue #3052

This repo demonstrates that [Azure/azure-functions-dotnet-worker#3052](https://github.com/Azure/azure-functions-dotnet-worker/issues/3052) — requesting `Action<IFunctionsWorkerApplicationBuilder>` overloads for `ConfigureFunctionsWebApplication` — is **not needed** with the current API.

## The issue's claim

The issue states that F# users who need to manually call extension startup `Configure()` methods (as a workaround for [#1246](https://github.com/Azure/azure-functions-dotnet-worker/issues/1246)) cannot do so with `FunctionsApplication.CreateBuilder()`, and must resort to a reflection hack to access the internal `HostBuilder` property. It also claims this makes .NET Aspire's `AddServiceDefaults()` harder to use.

## Why it already works

`FunctionsApplicationBuilder` (returned by `FunctionsApplication.CreateBuilder()`) implements **both**:

- `IFunctionsWorkerApplicationBuilder` — so it can be passed directly to `WorkerExtensionStartup.Configure()`
- `IHostApplicationBuilder` — so Aspire's `AddServiceDefaults()` works on it directly

## This repo proves it

This is a real .NET Aspire project (`dotnet new aspire`) with an F# Azure Functions app that does exactly what the issue asks for, **without** the proposed overloads and **without** any reflection hack:

```fsharp
let builder = FunctionsApplication.CreateBuilder(args)

FunctionsApplicationBuilderAspNetCoreExtensions.ConfigureFunctionsWebApplication(builder) |> ignore

// Pass builder directly — no reflection needed
ServiceBusExtensionStartup().Configure(builder)

// Aspire service defaults — works because builder is IHostApplicationBuilder
builder.AddServiceDefaults() |> ignore

builder.Build().Run()
```

## Structure

| Project | Description |
|---------|-------------|
| `VerifyIssue3052.AppHost` | Aspire orchestrator |
| `VerifyIssue3052.ServiceDefaults` | Aspire service defaults (template-generated) |
| `VerifyIssue3052.FuncApp` | F# Azure Functions app proving the point |

## Build

```bash
dotnet build
```

Requires .NET 10 SDK and Aspire workload (`dotnet workload install aspire`).
