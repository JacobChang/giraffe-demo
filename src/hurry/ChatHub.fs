namespace Hurry
open System.Threading.Tasks

module ChatHub =
    open System
    open Microsoft.AspNetCore.SignalR;
    open FSharp.Control.Tasks
    open Hurry.ChatSystem.Interfaces
    open Orleans

    type ChatHub() =
        inherit Hub()
