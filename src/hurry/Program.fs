namespace Hurry
open Microsoft.AspNetCore.Http
module App =
    open System
    open System.IO
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Cors.Infrastructure
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open Giraffe

    open Hurry.ChatSystem
    open Orleans

    let webApp =
        choose [
            route "/api/v1/channels" >=>
                choose [
                    GET >=> Channel.query
                    POST >=> Channel.create
                ]
            setStatusCode 404 >=> text "Not Found" ]

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message


    // ---------------------------------
    // Config and Main
    // ---------------------------------

    let configureCors (builder : CorsPolicyBuilder) =
        builder.WithOrigins("http://localhost:9091")
               .AllowAnyMethod()
               .AllowAnyHeader()
               |> ignore

    let configureApp (app : IApplicationBuilder) =
        app.UseGiraffeErrorHandler(errorHandler)
            .UseCors(configureCors)
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseSignalR(fun options -> 
                let pathStr = PathString("/chathub")
                options.MapHub<ChatHub.ChatHub>(pathStr)
            )
            .UseGiraffe(webApp)

    let configureLogging (builder : ILoggingBuilder) =
        builder.AddFilter(fun l -> l.Equals LogLevel.Error)
               .AddConsole()
               .AddDebug() |> ignore

    [<EntryPoint>]
    let main _ =
        let siloHost = SiloHost.build ()
        SiloHost.start siloHost

        let chatClient = Client.build ()
        Client.connect chatClient

        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot     = Path.Combine(contentRoot, "../webapp/build")

        let configureServices (services : IServiceCollection) =
            services.AddCors()    |> ignore
            services.AddGiraffe() |> ignore
            services.AddSignalR() |> ignore
            services.AddSingleton<IClusterClient>(chatClient) |> ignore

        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRoot)
            .UseIISIntegration()
            .UseWebRoot(webRoot)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()

        Client.close chatClient

        SiloHost.stop siloHost

        0
