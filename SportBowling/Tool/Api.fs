module Api

open System.Net
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open FSharp.Json

module Bits =
    type Client(apiKey, noCheckCertificate : bool option, proxy : string option) =
        [<Literal>]
        let root = "https://api.swebowl.se/api/v1"
        let customizeRequest (req : HttpWebRequest) =
            proxy |> function
            | Some s -> req.Proxy <- WebProxy(s, true)
            | _ -> ()
            noCheckCertificate |> function
            | Some b when b ->
                req.ServerCertificateValidationCallback <-
                    fun _sender _certificate _chain _policyErrors -> true
            | _ -> ()
            req

        let performGet url =
            Http.RequestString(
                url = url,
                httpMethod = HttpMethod.Get,
                headers = [
                    Referer "https://bits.swebowl.se"
                ],
                customizeHttpRequest = customizeRequest)

        let performPost url body =
            Http.RequestString(
                url = url,
                httpMethod = HttpMethod.Post,
                body = TextRequest (Json.serialize body),
                headers = [
                    Referer "https://bits.swebowl.se"
                    ContentType "application/json"
                ],
                customizeHttpRequest = customizeRequest)

        /// https://api.swebowl.se/api/v1/matchResult/GetMatchScores
        member _.GetMatchScores (Domain.MatchId matchId) =
            let matchScoresResponse =
                performGet $"%s{root}/matchResult/GetMatchScores?APIKey=%s{apiKey}&matchId=%d{matchId}"
                |> Contracts.Bits.MatchScores.Parse
            matchScoresResponse

        /// https://api.swebowl.se/api/v1/matchResult/GetMatchResults
        member _.GetMatchResults (Domain.MatchId matchId) (Domain.MatchSchemeId matchSchemeId) =
            let matchResultsResponse =
                performGet $"%s{root}/matchResult/GetMatchResults?APIKey=%s{apiKey}&matchSchemeId=%s{matchSchemeId}&matchId=%d{matchId}"
                |> Contracts.Bits.MatchResults.Parse
            matchResultsResponse

        /// https://api.swebowl.se/api/v1/Division
        member _.GetDivision (Domain.SeasonId seasonId) =
            let divisionResponse =
                performGet
                    $"%s{root}/Division?APIKey=%s{apiKey}&teamId=0&countyId=-1&seasonId=%d{seasonId}"
                |> Contracts.Bits.Divisions.Parse
            divisionResponse

        /// https://api.swebowl.se/api/v1/Match
        member _.GetMatch (Domain.DivisionId divisionId) (Domain.SeasonId seasonId) =
            let matchResponse =
                performGet
                    $"%s{root}/Match/?APIKey=%s{apiKey}&divisionId=%d{divisionId}&seasonId=%d{seasonId}"
                |> Contracts.Bits.Match.Parse
            matchResponse

        /// https://api.swebowl.se/api/v1/matchResult/GetHeadInfo
        member _.GetHeadInfo (Domain.MatchId matchId) =
            let headInfoResponse =
                performGet
                    $"%s{root}/matchResult/GetHeadInfo?APIKey=%s{apiKey}&id=%d{matchId}"
                |> Contracts.Bits.HeadInfo.Parse
            headInfoResponse
