namespace MobileTestingApp

open System
open System.Collections.Generic

type Gender = Male | Female

type Student = {
    Id : Guid
    FirstName : string
    LastName : string
    Gender : Gender
    Email : string
}

module Db =

    let studentDb = new Dictionary<Guid, Student>()
    let getStudent () =
        studentDb.Values :> seq<Student>

    let createStudent student =
        let id = Guid.NewGuid()
        let newStudent = {student with Id = id}
        studentDb.Add(id, newStudent)

    let upload students = 
        students |> Seq.iter(createStudent)
