module Model

open System
open Bolero


/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter">] Counter
    | [<EndPoint "/data">] Data

/// The Elmish application's model.
type Model =
    { page: Page
      counter: int
      books: Book[] option
      error: string option }

and Book =
    { title: string
      author: string
      publishDate: DateTime
      isbn: string }

let initModel =
    { page = Home
      counter = 0
      books = None
      error = None }

module Messages =

    open System.Net.Http
    open System.Net.Http.Json
    open Elmish

    /// The Elmish application's update messages.
    type Message =
        | SetPage of Page
        | Increment
        | Decrement
        | SetCounter of int
        | GetBooks
        | GotBooks of Book[]
        | Error of exn
        | ClearError

    let router = Router.infer SetPage (fun model -> model.page)

    let update (http: HttpClient) message model =
        match message with
        | SetPage page ->
            { model with page = page }, Cmd.none

        | Increment ->
            { model with counter = model.counter + 10 }, Cmd.none
        | Decrement ->
            { model with counter = model.counter - 1 }, Cmd.none
        | SetCounter value ->
            { model with counter = value }, Cmd.none

        | GetBooks ->
            let getBooks() = http.GetFromJsonAsync<Book[]>("/books.json")
            let cmd = Cmd.OfTask.either getBooks () GotBooks Error
            { model with books = None }, cmd
        | GotBooks books ->
            { model with books = Some books }, Cmd.none

        | Error exn ->
            { model with error = Some exn.Message }, Cmd.none
        | ClearError ->
            { model with error = None }, Cmd.none    
