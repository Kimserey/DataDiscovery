#r "../packages/Deedle/lib/net40/Deedle.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "../packages/FSharp.Charting/FSharp.Charting.fsx"

open System
open FSharp.Data
open FSharp.Charting
open System
open Deedle

/// Loads the csv and use the column Id from the csv as index
let dfSalaries =
    Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/data/Salaries.csv")
    |> Frame.indexRowsInt "Id"

/// Look at few lines to see what are the data and columns
dfSalaries
|> Frame.denseRows
|> Series.take 10
|> Frame.ofRows

/// Select specific columns and rename to our taste
let df =
    dfSalaries.Columns.[ [ "JobTitle"; "BasePay"; "OvertimePay"; "OtherPay"; "Benefits"; "TotalPay"; "TotalPayBenefits"; "Year" ] ]
    |> Frame.indexColsWith [ "Job"; "Base"; "Overtime"; "Other"; "Benefits"; "Net"; "Gross"; "Year" ]

let jobCol =
    df.GetColumn<string>("Job")
    |> Series.mapValues (fun x -> 
        x.ToUpperInvariant()
         .Replace("1", "I")
         .Replace("2", "II")
         .Replace("3", "III")
         .Trim())
df?Job <- jobCol

/// Group jobs by title, the result is a 2 level frame, 1rst level being the Job and 2nd level being the automatic index
let dfByJob =
    df |> Frame.groupRowsByString "Job"

/// Group by year, we need to flatten the year otherwise it is a tuple of tuple
let dfByJobByYear =
    dfByJob 
    |> Frame.groupRowsByString "Year"
    |> Frame.mapRowKeys Pair.flatten3

let ``mean gross salary per year`` =
    /// level is used to specify at which level of the frame do we want to apply the stats
    /// get1Of3 means we calculate the stats at the level 1 (Year)
    let means =
        dfByJobByYear
        |> Frame.getNumericCols
        |> Series.dropMissing
        |> Series.mapValues (Stats.levelMean Pair.get1Of3)
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