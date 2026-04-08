open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Builder
open Microsoft.Azure.Functions.Worker.Extensions.ServiceBus
open Microsoft.Extensions.Hosting

[<EntryPoint>]
let main args =
    let builder = FunctionsApplication.CreateBuilder(args)

    // Configure ASP.NET Core integration
    FunctionsApplicationBuilderAspNetCoreExtensions.ConfigureFunctionsWebApplication(builder) |> ignore

    // Issue #3052 workaround: manually call extension startup for F# (issue #1246).
    // No reflection hack needed — builder implements IFunctionsWorkerApplicationBuilder directly.
    ServiceBusExtensionStartup().Configure(builder)

    // .NET Aspire service defaults — works because builder implements IHostApplicationBuilder.
    builder.AddServiceDefaults() |> ignore

    builder.Build().Run()
    0
