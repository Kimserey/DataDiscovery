#load "Load.fsx"

open System
open FSharp.Data
open FSharp.Charting
open System
open Deedle
open DataDiscovery.Root

/// base against other pay

let dfByYear =
    df.Columns.[ [ "Year"; "Base"; "Overtime"; "Other" ] ]
    |> Frame.groupRowsByInt "Year"
   
let frames  =
    [ 2011; 2012; 2013; 2014 ]
    |> List.map (fun year -> 
        string year,
        dfByYear.Rows.[ year, * ]
        |> Frame.rows
        |> Series.observations)

/// Plot base against overtime
///
frames
|> List.map (fun (title, df) ->
    let data =
        df 
        |> Seq.map (fun (_, values) -> values?Base, values?Overtime)
        |> Seq.filter (fun (b, o) -> b >= 0. && o >= 0.)

    Chart.FastPoint(data, 
                    Title = title, 
                    XTitle = "Base", 
                    YTitle = "Overtime" ))
|> Chart.Rows

/// Plot base against other payments 
///
frames
|> List.map (fun (title, df) ->
    let data =
        df 
        |> Seq.map (fun (_, values) -> values?Base, values?Other)
        |> Seq.filter (fun (b, o) -> b >= 0. && o >= 0.)

    Chart.FastPoint(data, 
                    Title = title, 
                    XTitle = "Base", 
                    YTitle = "Other" ))
|> Chart.Rows
