#r "../packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"
#r "../packages/Deedle/lib/net40/Deedle.dll"

open System
open FSharp.Charting
open Deedle

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