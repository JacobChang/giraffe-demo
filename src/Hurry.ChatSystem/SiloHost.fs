namespace Hurry.ChatSystem

module SiloHost =
    open Microsoft.Extensions.Logging
    open Orleans
    open Orleans.Runtime.Configuration
    open Orleans.Hosting
    open System
    open FSharp.Control.Tasks

    open Hurry.ChatSystem.Grains
    open Hurry.ChatSystem.Interfaces

    let build () =
          let builder = SiloHostBuilder()
          builder
            .UseLocalhostClustering()
            .ConfigureApplicationParts(fun parts ->
                parts.AddApplicationPart(typeof<WebSocketAgentGrain>.Assembly)
                      .AddApplicationPart(typeof<IWebSocketAgent>.Assembly)
                      .WithCodeGeneration() |> ignore)
            .ConfigureLogging(fun logging -> logging.AddConsole() |> ignore)
            .Build()

    let start (host: ISiloHost) =
        let t = task {
            do! host.StartAsync ()
            printfn "SiloHost is started"
        }
        t.Wait()

    let stop (host: ISiloHost) =
        let t = task {
            do! host.StopAsync()
            printfn "SiloHost is stopped"
        }
        t.Wait()