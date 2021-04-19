module Contracts

open FSharp.Data

module Bits =
    type MatchResults = JsonProvider<"Json/MatchResults.json", InferTypesFromValues = true, RootName = "MatchResult">
    type MatchScores = JsonProvider<"Json/MatchScores.json", InferTypesFromValues = true, RootName = "MatchScore">

    /// [
    ///   {
    ///     "divisionId": "1",
    ///     "divisionName": "Elitserien Herrar"
    ///   }
    /// ]
    type Divisions = JsonProvider<"Json/Division.json", InferTypesFromValues = true, RootName = "Division">
    type Match = JsonProvider<"Json/Match.json", InferTypesFromValues = true, RootName = "Match">
    type HeadInfo = JsonProvider<"Json/HeadInfo.json", InferTypesFromValues = true, RootName = "HeadInfo">

type Method = Post | Get
type RequestDefinition = {
    Url : string
    Method : Method
    Headers : (string * string) list
    Body : string option
}
type RequestStringAsync = RequestDefinition -> Async<string>

// https://stackoverflow.com/questions/5569909/how-do-i-create-an-f-function-with-a-printf-style-logging-argument
type Logger =
    abstract Log : fmt : Printf.StringFormat<'a, unit> -> 'a
    // abstract LogException : e : System.Exception -> fmt : Printf.StringFormat<'a, unit> -> 'a
