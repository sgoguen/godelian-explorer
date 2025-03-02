module Model

open System
open Bolero


/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter">] Counter
    | [<EndPoint "/data">] Data

[<AbstractClass>]
type State() =
    abstract member SendMessage: obj -> unit

type State<'Value, 'Message>(initialValue: 'Value, update: 'Message -> 'Value -> 'Value) =
    inherit State()
    let mutable state = initialValue
    member this.Update(message: 'Message) = state <- update message state

    override this.SendMessage(message: obj) =
        match message with
        | :? 'Message as msg -> this.Update msg
        | _ -> failwithf "Invalid message type: %O" message

module Counter =
    type CounterModel = { count: int }

    let initCounterModel = { count = 0 }

    type CounterMessage =
        | Incr
        | Decr
        | SetCount of int

    let update message model =
        match message with
        | Incr -> { model with count = model.count + 1 }
        | Decr -> { model with count = model.count - 1 }
        | SetCount value -> { model with count = value }

/// The Elmish application's model.
type Model =
    { page: Page
      counter: Counter.CounterModel
      //   counter: int
      books: Book[] option
      error: string option
      miscState: Map<string, State> }

and Book =
    { title: string
      author: string
      publishDate: DateTime
      isbn: string }

let initModel =
    { page = Home
      counter = Counter.initCounterModel
      books = None
      error = None
      miscState = Map.empty }

module Messages =

    open System.Net.Http
    open System.Net.Http.Json
    open Elmish

    /// The Elmish application's update messages.
    type Message =
        | SetPage of Page
        | CounterMsg of Counter.CounterMessage
        | GetBooks
        | GotBooks of Book[]
        | Error of exn
        | ClearError

    let router = Router.infer SetPage (fun model -> model.page)

    let update (http: HttpClient) message model =
        match message with
        | SetPage page -> { model with page = page }, Cmd.none
        | GetBooks ->
            let getBooks () =
                http.GetFromJsonAsync<Book[]>("/books.json")

            let cmd = Cmd.OfTask.either getBooks () GotBooks Error
            { model with books = None }, cmd
        | GotBooks books -> { model with books = Some books }, Cmd.none

        | Error exn -> { model with error = Some exn.Message }, Cmd.none
        | ClearError -> { model with error = None }, Cmd.none
        | CounterMsg msg ->
            let c2 = Counter.update msg model.counter
            { model with counter = c2 }, Cmd.none
