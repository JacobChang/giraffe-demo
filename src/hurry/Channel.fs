module Channel

open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open Giraffe

let query (next : HttpFunc) (ctx : HttpContext) =
    task {
        return! next ctx
    }

let create (next : HttpFunc) (ctx : HttpContext) =
    task {
        return! next ctx
    }
