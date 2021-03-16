module Database

open System
open Npgsql
open Npgsql.FSharp

module Entities =
    type Division = {
        DivisionId : int
        ExternalDivisionId : string
        DivisionName : string
    }

    type HttpMethod = Get | Post
    type Request = {
        RequestId : int
        Url : string
        Method : HttpMethod
        Body : string option
    }

type DatabaseConnection = {
    Host : string
    Database : string
    Username : string
    Password : string
}
with
    member this.Format() =
        $"Host=%s{this.Host};Database=%s{this.Database};Username=%s{this.Username};Password=%s{this.Password}"

type Gateway(connection : NpgsqlConnection) =

    let run f =
        use transaction = connection.BeginTransaction()
        let result =
            connection
            |> FSharp.Sql.existingConnection
            |> f
        transaction.Commit()
        result

    member _.StoreDivision
        (bitsDivisions : Contracts.Bits.Divisions.Division array) =

        let insertDivision connection (division : Contracts.Bits.Divisions.Division) =
            connection
            |> Sql.existingConnection
            |> Sql.query "INSERT INTO bits.division \
                    (external_division_id, division_name, created_utc) \
                 VALUES (@external_division_id, @division_name, @created_utc)"
            |> Sql.parameters [
                "@external_division_id", Sql.int division.DivisionId
                "@division_name", Sql.string division.DivisionName
                "@created_utc", Sql.timestamp DateTime.UtcNow
            ]
            |> Sql.executeNonQuery
            |> function
            | Ok items -> printfn "Stored %d items" items
            | Error err -> raise err

        use transaction = connection.BeginTransaction()
        bitsDivisions
        |> Array.iter (insertDivision connection)
        transaction.Commit()

    member _.StoreRequest
        url
        (method : Entities.HttpMethod)
        body =
        run
        <| FSharp.Sql.query
            "INSERT INTO bits.request \
                (url, method, body, created_utc) \
             VALUES (@url, @method, @body, @created_utc)"
        |> FSharp.Sql.parameters
            [
                "@url", Sql.string url
                "@method", Sql.string (method.ToString())
                "@body", Sql.string body
                "@created_utc", Sql.timestamp DateTime.UtcNow
            ]
        |> FSharp.Sql.executeNonQuery
        |> function
        | Ok items -> printf "Stored %d items" items
        | Error err -> raise err
