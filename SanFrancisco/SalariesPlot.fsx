#load "Load.fsx"

open System
open FSharp.Data
open FSharp.Charting
open System
open Deedle
open DataDiscovery.Root

let ``mean gross salary per year`` =
    /// level is used to specify at which level of the frame do we want to apply the stats
    /// get1Of2 means we calculate the stats at the level 1 (Year)
    let means =
        df
        |> Frame.groupRowsByString "Year"
        |> Frame.getNumericCols
        |> Series.dropMissing
        |> Series.mapValues (Stats.levelMean Pair.get1Of2)
        |> Frame.ofColumns
 
    means?Gross
    |> Series.observations
    |> Seq.map (fun (k, v) -> int k, v)
    |> Seq.toList

/// Get only year and gross from the dataset
/// Filter positive values
let dfGrossPerYear =
    df.Columns.[ [ "Year"; "Gross" ] ]
    |> Frame.groupRowsByString "Year"
    |> Frame.filterRowValues (fun r -> r?Gross > 0.)
    |> Frame.dropCol "Year"
        
let ``Gross salaries per year`` =
    let gross = 
        (dfGrossPerYear |> Frame.getNumericCols |> Series.dropMissing)?Gross

    gross
    |> Series.observations
    |> Seq.map (fun ((year, _), value) -> int year, value)
    |> Seq.toList

Chart.Combine ([ Chart.FastPoint(``Gross salaries per year``, Name = "Gross salaries per year")
                 Chart.Line(``mean gross salary per year``, Name = "Mean gross salary per year") ])