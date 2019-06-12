module WebSocketMiddleware

open System
open System.Text
open System.Threading
open System.Threading.Tasks
open System.Net.WebSockets
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open ChatSystem

type WebSocketMiddleware(next : RequestDelegate) =
    member __.Invoke(ctx : HttpContext) = task {
        match ctx.WebSockets.IsWebSocketRequest with
        | true ->
            let serviceProvider = ctx.RequestServices
            let t = typeof<ChatSystem>
            let chatSystem = serviceProvider.GetService(t)
            let! webSocket = ctx.WebSockets.AcceptWebSocketAsync() |> Async.AwaitTask
            let socketFinishedTcs = TaskCompletionSource()
            return! socketFinishedTcs.Task
        | false ->
            return! next.Invoke(ctx)
    }
