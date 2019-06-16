namespace Hurry

module Channel =
    open FSharp.Control.Tasks.V2
    open Microsoft.AspNetCore.Http
    open Giraffe
    open Orleans
    open Hurry.ChatSystem.Interfaces

    let channelManagerPrimaryKey = "channel-manager"

    let query (next : HttpFunc) (ctx : HttpContext) =
        task {
            let chatClient = ctx.GetService<IClusterClient>()
            let channelManager = chatClient.GetGrain<IChannelManager>(channelManagerPrimaryKey)
            let! channels = channelManager.query ()
            return! json channels next ctx
        }

    [<CLIMutable>]
    type ChannelCreateParams =
        {
            title: string
            duration: uint32
        }

    let create (next : HttpFunc) (ctx : HttpContext) =
        task {
            let chatClient = ctx.GetService<IClusterClient>()
            let channelManager = chatClient.GetGrain<IChannelManager>(channelManagerPrimaryKey)
            let! createParams = ctx.BindJsonAsync<ChannelCreateParams>()
            let! channel = channelManager.create createParams.title createParams.duration
            return! json channel next ctx
        }
