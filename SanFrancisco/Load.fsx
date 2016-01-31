module DataDiscovery.Root

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