module WebSocketMiddleware

open System
open System.Text
open System.Threading
open System.Threading.Tasks
open System.Net.WebSockets
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Hurry.ChatSystem.Interfaces
open Orleans

type WebSocketMiddleware(next : RequestDelegate) =
    member __.Invoke(ctx : HttpContext) = task {
        match ctx.WebSockets.IsWebSocketRequest with
        | true ->
            let serviceProvider = ctx.RequestServices
            let chatClient: IClusterClient = ctx.RequestServices.GetService(typeof<IClusterClient>) :?> IClusterClient
            let! webSocket = ctx.WebSockets.AcceptWebSocketAsync() |> Async.AwaitTask
            let agent = chatClient.GetGrain<IWebSocketAgent> (Guid.NewGuid())

            return! agent.handle webSocket
        | false ->
            return! next.Invoke(ctx)
    }
