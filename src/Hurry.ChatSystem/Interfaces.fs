namespace Hurry.ChatSystem

module Interfaces =
    open System.Net.WebSockets
    open System.Threading.Tasks

    type IWebSocketAgent = 
        inherit Orleans.IGrainWithGuidCompoundKey
        abstract member handle : WebSocket -> Task<unit>

    type IWebSocketReceiver =
        inherit Orleans.IGrainWithGuidCompoundKey

    type IWebSocketSender =
        inherit Orleans.IGrainWithGuidCompoundKey

    type IWebSocketChannel =
        inherit Orleans.IGrainWithGuidCompoundKey
