﻿// dotnet watch run --no-restore -- --api-key 123 --season-id 2006

open System
open Argu

type Arguments =
    | [<CliPrefix(CliPrefix.None)>] Fetch_Matches of ParseResults<FetchMatchesArguments>
    | [<CliPrefix(CliPrefix.None)>] Migrate_Database of ParseResults<MigrateDatabaseArguments>
    | [<CliPrefix(CliPrefix.None)>] Scratch of ParseResults<ScratchArguments>
    | Debug_Http
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Fetch_Matches _ -> "Fetch matches."
            | Migrate_Database _ -> "Migrate database."
            | Debug_Http -> "Debug HTTP."
            | Scratch _ -> "Scratch program."
and FetchMatchesArguments =
    | [<Mandatory>] Api_Key of apiKey : string
    | [<Mandatory>] Season_Id of seasonId : int
    | No_Check_Certificate
    | Proxy of proxy : string
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Api_Key _ -> "Specifies the ApiKey."
            | Season_Id _ -> "specifies the season id."
            | No_Check_Certificate -> "Don't check the server certificate against the available certificate authorities."
            | Proxy _ -> "Specifies the proxy to use."
and MigrateDatabaseArguments =
    | [<Mandatory>] Host of host : string
    | [<Mandatory>] Database of database : string
    | [<Mandatory>] Username of username : string
    | [<Mandatory>] Password of password : string
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Host _ -> "Specifies the database host."
            | Database _ -> "Specifies the database name."
            | Username _ -> "Specifies the username"
            | Password _ -> "Specifies the password."
and ScratchArguments =
    | Verbose
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Verbose -> "Verbose output."

let fetchMatches (args : ParseResults<FetchMatchesArguments>) =
    let apiKey = args.GetResult Api_Key
    let seasonId = args.GetResult Season_Id
    let noCheckCertificate =
        if args.Contains(No_Check_Certificate) then Some true else None
    let proxy = args.TryGetResult(Proxy)
    let bitsClient = Api.Bits.Client(apiKey, noCheckCertificate, proxy)
    let workflow = Workflows.FetchMatches()
    workflow.Run bitsClient (Domain.SeasonId seasonId)

let migrateDatabase (args : ParseResults<MigrateDatabaseArguments>) =
    let host = args.GetResult Host
    let database = args.GetResult Database
    let username = args.GetResult Username
    let password = args.GetResult Password
    let workflow = Workflows.MigrateDatabase()
    workflow.Run host database username password

module Scratch =
    open Hopac

    let Run (_ : ParseResults<ScratchArguments>) =
        let longerHelloWorldJob = job {
          do! timeOutMillis 2000
          printfn "Hello, World!"
        }
        run longerHelloWorldJob
        0

let run argv =
    let parser = ArgumentParser.Create<Arguments>(
                    programName = AppDomain.CurrentDomain.FriendlyName)
    let results = parser.ParseCommandLine argv

    let debugHttp = results.Contains(Debug_Http)
    let _l =
        if debugHttp then Some (new Infrastructure.LoggingEventListener()) else None

    match results.GetSubCommand() with
    | Fetch_Matches args -> fetchMatches args
    | Migrate_Database args -> migrateDatabase args
    | Scratch args -> Scratch.Run args
    | Debug_Http -> failwith "Unexpected"

[<EntryPoint>]
let main argv =
    try
        run argv
    with
        | :? ArguException as ex ->
            eprintfn "%s" ex.Message
            1
        | ex ->
            eprintfn "%A" ex
            1
