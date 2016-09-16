namespace MobileTestingApp

// see http://blog.tamizhvendan.in/blog/2015/06/11/building-rest-api-in-fsharp-using-suave/
open System.IO

module App =
    open Suave.Web
    open Suave

    [<EntryPoint>]
    let main argv =   
        printfn "Running!" 
        File.ReadAllLines(System.Environment.CurrentDirectory + "\Start.txt") |> Seq.iter(fun line -> printfn "%s" line)

        let students = routes "students" {
            GetAll = Db.getStudent
            Create = Db.createStudent
            Upload = Db.upload
        }
       
        let app = choose[students;] 

        startWebServer defaultConfig app
            
        0
