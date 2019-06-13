namespace Hurry.ChatSystem

module Client =
    open System
    open System.Threading.Tasks
    open Microsoft.Extensions.Logging
    open Orleans
    open Orleans.Runtime.Configuration
    open Orleans.Hosting
    open FSharp.Control.Tasks

    open Hurry.ChatSystem.Interfaces

    let build () =
          let builder = ClientBuilder()
          builder
            .UseLocalhostClustering()
            .ConfigureApplicationParts(fun parts -> parts.AddApplicationPart((typeof<IWebSocketAgent>).Assembly).WithCodeGeneration() |> ignore )
            .ConfigureLogging(fun logging -> logging.AddConsole() |> ignore)
            .Build()

    let connect (client: IClusterClient) =
        let t = task {
            do! client.Connect( fun (ex: Exception) -> task {
                do! Task.Delay(1000)
                return true
            })
        }

        t.Wait()

    
    let close (client: IClusterClient) =
        let t = task {
            do! client.Close()
        }

        t.Wait()
