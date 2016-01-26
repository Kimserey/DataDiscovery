#r "../packages/Deedle/lib/net40/Deedle.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "../packages/FSharp.Charting/FSharp.Charting.fsx"

open System
open FSharp.Data
open FSharp.Charting

open System
open Deedle

let dfSalaries =
    Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/data/Salaries.csv")

let df =
    dfSalaries.Columns.[ [ "JobTitle"; "BasePay"; "OvertimePay"; "OtherPay"; "Benefits"; "TotalPay"; "TotalPayBenefits" ] ]
    |> Frame.indexColsWith [ "Job"; "Base"; "Overtime"; "Other"; "Benefits"; "Total"; "TotalBenefitAdded" ]

let jobCol =
    df.GetColumn<string>("Job")
    |> Series.mapValues (fun x -> 
        x.ToUpperInvariant()
         .Replace("1", "I")
         .Replace("2", "II")
         .Replace("3", "III")
         .Trim())
df?Job <- jobCol

let dfGroupedByJob =
    df 
    |> Frame.groupRowsByString "Job"
dfGroupedByJob.Print()

/// level is used to specify at which level of the frame do we want to apply the stats
/// get1Of2 would mean at the JOB level since the key is string*int
let means =
    dfGroupedByJob
    |> Frame.getNumericCols
    |> Series.dropMissing
    |> Series.mapValues (Stats.levelMean Pair.get1Of2)
    |> Frame.ofColumns
means.Print()

let moreThan3000 =
    df.GetColumn<string>("Job")
    |> Series.groupBy (fun _ -> id) 
    |> Series.mapValues (Stats.count)
    |> Series.filterValues (fun v -> v > 3000)
moreThan3000.Print()