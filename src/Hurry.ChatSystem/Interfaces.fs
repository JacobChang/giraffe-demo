namespace Hurry.ChatSystem
open System.Threading
open System

module Interfaces =
    open System.Net.WebSockets
    open System.Threading.Tasks

    type IWebSocketAgent = 
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member handle : WebSocket -> Task<unit>
        abstract member handleMessage: String -> Task<unit>
        abstract member handleClose: WebSocket -> Task<unit>

    type IWebSocketReceiver =
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member receive : WebSocket -> cancellationToken: CancellationToken ->  Task<unit>

    type IWebSocketSender =
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member send : WebSocket -> message: string -> cancellationToken: CancellationToken -> Task<unit>

    type IWebSocketChannel =
        inherit Orleans.IGrainWithGuidCompoundKey
