namespace Hurry.ChatSystem
open System.Threading
open System

module Interfaces =
    open System.Net.WebSockets
    open System.Threading.Tasks

    type IWebSocketAgent = 
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member handle : WebSocket -> Task
        abstract member handleMessage: String -> Task<unit>
        abstract member handleClose: WebSocket -> Task<unit>

    type IWebSocketReceiver =
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member receive : WebSocket -> cancellationToken: CancellationToken ->  Task<unit>

    type IWebSocketSender =
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member send : WebSocket -> message: string -> cancellationToken: CancellationToken -> Task<unit>

    type Channel = {
        id: Guid
        title: string
        startTime: DateTime
        duration: uint32 }

    type IChannel =
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member join : agentId: Guid -> Task<unit>
        abstract member leave : agentid: Guid -> Task<unit>
        abstract member dismiss: unit -> Task<unit>
        abstract member broadcast: agentId: Guid -> message: string -> Task<unit>

    type IChannelManager =
        inherit Orleans.IGrainWithStringKey
        abstract member create: title: string -> duration: uint32 -> Task<Channel>
        abstract member query: unit -> Task<Channel[]>
        abstract member destroy: id: Guid -> Task<Channel option>
