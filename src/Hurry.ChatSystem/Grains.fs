namespace Hurry.ChatSystem

module Grains =
    open System.Threading.Tasks
    open Orleans
    open Hurry.ChatSystem.Interfaces
    open System.Net.WebSockets

    type WebSocketAgentGrain () =
        inherit Grain ()
        interface IWebSocketAgent with 
            member this.handle (webSocket : WebSocket) : Task<unit> = 
                let taskCompleteSource = TaskCompletionSource()

                taskCompleteSource.Task