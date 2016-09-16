namespace MobileTestingApp

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful


[<AutoOpen>]
module routing =    
    open Suave.RequestErrors
    open Suave.Filters
    

    // 'a -> WebPart
    let JSON v =     
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    
        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK 
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a  

    let getResourceFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        Create : 'a -> unit // might be nicer to return the thing you have made 
        Upload : 'a seq -> unit
    }

    let routes resourceName resource =
       
        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        
        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll= warbler (fun _ -> resource.GetAll () |> JSON)

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)
            ]
        ]