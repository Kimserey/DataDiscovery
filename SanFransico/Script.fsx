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

// Create from sequence of keys and sequence of values
let dates  = 
  [ DateTime(2013,1,1); 
    DateTime(2013,1,4); 
    DateTime(2013,1,8) ]
let values = 
  [ 10.0; 20.0; 30.0 ]
let first = Series(dates, values)

/// Generate date range from 'first' with 'count' days
let dateRange (first:System.DateTime) count =
    [ for i in 0..(count - 1) -> first.AddDays(float i) ]

/// Generate 'count' number of random doubles
let rand count =
    let rnd = System.Random()
    [ for i in 0..(count-1) -> rnd.NextDouble() ]

// A series with values for 10 days 
let second = Series(dateRange (DateTime(2013,1,1)) 10, rand 10)

let df1 = Frame(["first"; "second"], [ first; second ])

// The same as previously
let df2 = Frame.ofColumns ["first" => first; "second" => second]
df2.Print()

// Transposed - here, rows are "first" and "second" & columns are dates
let df3 = Frame.ofRows ["first" => first; "second" => second]
df3.Print()

let df4 = 
  [ ("Monday", "Tomas", 1.0); ("Tuesday", "Adam", 2.1)
    ("Tuesday", "Tomas", 4.0); ("Wednesday", "Tomas", -5.4) ]
  |> Frame.ofValues
df4.Print()

// Assuming we have a record 'Price' and a collection 'values'
type Price = { Day : DateTime; Open : float }
let prices = 
  [ { Day = DateTime.Now; Open = 10.1 }
    { Day = DateTime.Now.AddDays(1.0); Open = 15.1 }
    { Day = DateTime.Now.AddDays(2.0); Open = 9.1 } ]

// Creates a data frame with columns 'Day' and 'Open'
let df5 = Frame.ofRecords prices
df5.Print()