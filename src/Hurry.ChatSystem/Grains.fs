namespace Hurry.ChatSystem
open System
open System.Threading
open System.Collections.Generic
open System.Text

module Grains =
    open System.Threading.Tasks
    open Orleans
    open Hurry.ChatSystem.Interfaces
    open System.Net.WebSockets

    type WebSocketAgentGrain () =
        inherit Grain ()

        let mutable currWebSocket: WebSocket = null;
        let mutable taskCompletionSource: TaskCompletionSource<unit> = null
        let mutable receiverCancallationSource: CancellationTokenSource = null
        let mutable senderCancallationSource: CancellationTokenSource = null

        interface IWebSocketAgent with 
            member this.handle (webSocket : WebSocket) : Task =
                printfn "start %A" webSocket
                let closeTask = (this :> IWebSocketAgent).handleClose currWebSocket
                closeTask.Wait()

                currWebSocket <- webSocket
                let primaryKey = this.GetPrimaryKey()
                receiverCancallationSource <- new CancellationTokenSource()
                senderCancallationSource <- new CancellationTokenSource()

                let receiver = this.GrainFactory.GetGrain<IWebSocketReceiver>(primaryKey, "receiver")
                receiver.receive webSocket receiverCancallationSource.Token |> ignore

                taskCompletionSource <- TaskCompletionSource()
                taskCompletionSource.Task :> Task

            member this.handleMessage (message: string) : Task<unit> =
                let primaryKey = this.GetPrimaryKey()
                let sender = this.GrainFactory.GetGrain<IWebSocketSender>(primaryKey, "sender")
                let task = sender.send currWebSocket message senderCancallationSource.Token
                task.Wait()

                Task.FromResult ()

            member this.handleClose (webSocket : WebSocket) : Task<unit> =
                if isNull taskCompletionSource |> not then
                    receiverCancallationSource.Cancel()
                    receiverCancallationSource <- null
                    senderCancallationSource.Cancel()
                    senderCancallationSource <- null
                    taskCompletionSource.SetResult()
                    currWebSocket <- null

                Task.FromResult ()
                
    type WebSocketReceiverGrain() =
        inherit Grain()

        interface IWebSocketReceiver with
            member this.receive (webSocket: WebSocket) (cancellationToken: CancellationToken) =
                let mutable response: WebSocketReceiveResult = null
                let message = List<byte>()
                let buffer: byte[] =  Array.zeroCreate 4096

                let mutable stop = false
                while not stop do
                    try
                        let task = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        task.Wait()
                        let response = task.Result
                        message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count))
                        if response.EndOfMessage then
                            let primaryKey = this.GetPrimaryKey()
                            let agent = this.GrainFactory.GetGrain<IWebSocketAgent>(primaryKey, "agent")
                            agent.handleMessage (Encoding.UTF8.GetString(message.ToArray())) |> ignore
                    with
                    | exp -> stop <- true

                Task.FromResult()

    type WebSocketSenderGrain() =
        inherit Grain()

        interface IWebSocketSender with
            member this.send (webSocket: WebSocket) (message: string) (cancellationToken: CancellationToken) =

                Task.FromResult()

    type Channel() =
        inherit Grain()

    type ChannelManager() =
        inherit Grain()

        let mutable channels = Map.empty

        interface IChannelManager with
            member this.create (title: string) (duration: uint32) =
                let id = Guid.NewGuid()
                let channel = {
                    id = id
                    title = title
                    startTime = DateTime.Now
                    duration = duration
                }
                channels <- Map.add id channel channels
                Task.FromResult channel

            member this.query () =
                let result =
                    Map.toArray channels
                    |> Array.map snd
                Task.FromResult result

            member this.destroy (id: Guid) =
                let channel = Map.tryFind id channels
                Task.FromResult channel
