module Channel

open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open Giraffe
open Orleans

let query (next : HttpFunc) (ctx : HttpContext) =
    task {
        let chatClient = ctx.GetService<IClusterClient>()
        return! next ctx
    }

let create (next : HttpFunc) (ctx : HttpContext) =
    task {
        let chatClient = ctx.GetService<IClusterClient>()
        return! next ctx
    }
