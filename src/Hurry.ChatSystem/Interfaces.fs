namespace Hurry.ChatSystem

module Interfaces =
    open System.Net.WebSockets
    open System.Threading.Tasks

    type IWebSocketAgent = 
        inherit Orleans.IGrainWithGuidKey
        abstract member handle : WebSocket -> Task<unit>
