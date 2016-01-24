#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"

open System
open FSharp.Data
open FSharp.Charting

type Salaries = CsvProvider<"data/Salaries.csv">
type Salary = Salaries.Row

let sample =
    Salaries.GetSample()
    
let total = sample.Rows |> Seq.length

let lesserThan100 =
    sample.Rows
    |> Seq.groupBy (fun salary -> salary.JobTitle.ToUpperInvariant())
    |> Seq.filter (fun (_, group) -> group |> Seq.length < 100)
    |> Seq.sumBy (fun (_, g) -> g |> Seq.length)
    |> fun x -> float x / float total
    |> fun x -> Math.Round(x * 100., 2)

let moreThan100 =
    sample.Rows
    |> Seq.groupBy (fun salary -> salary.JobTitle.ToUpperInvariant())
    |> Seq.filter (fun (_, group) -> group |> Seq.length >= 100)
    |> Seq.sumBy (fun (_, g) -> g |> Seq.length)
    |> fun x -> float x / float total
    |> fun x -> Math.Round(x * 100., 2)

type SalaryGroup = {
    JobTitle: string
    Sample: Salary list
}

type Ranked<'T> = {
    Rank: int
    Item: 'T
}

let group =
    sample.Rows
    |> Seq.groupBy (fun salary -> salary.JobTitle.ToUpperInvariant())
    |> Seq.filter (fun (_, group) -> group |> Seq.length >= 100)
    |> Seq.map (fun (title, group) -> { JobTitle = title; Sample = group |> Seq.toList })
    |> Seq.sortBy (fun salaryGroup -> - (salaryGroup.Sample |> List.averageBy (fun s -> s.OvertimePay)))
    |> Seq.toList
    |> List.mapi (fun rank salaryGroup -> { Rank = rank; Item = salaryGroup })

printfn "%50s | %10s | %10s | %10s | %5s | %5s |" "TITLE" "BASE" "OVERTIME" "GROSS" "COUNT" "RANK"
group 
|> List.iter (fun ranked -> 
    printfn "%50s | %10.2f | %10.2f | %10.2f | %5i | %5i |" ranked.Item.JobTitle
        (ranked.Item.Sample |> List.averageBy (fun s -> s.BasePay))
        (ranked.Item.Sample |> List.averageBy (fun s -> s.OvertimePay))
        (ranked.Item.Sample |> List.averageBy (fun s -> s.TotalPayBenefits))
        (ranked.Item.Sample |> List.length)
        ranked.Rank)

#r "../packages/Deedle/lib/net40/Deedle.dll"

open Deedle

let dfSalaries =
    Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/data/Salaries.csv")
dfSalaries.Print()
