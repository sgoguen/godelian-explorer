namespace Views

module Counter =

    open Bolero.Html
    open Bolero.Virtualize

    open Model
    open Model.Messages

    open Model.Counter

    open GodelianToolkit
    open GodelianToolkit.AutoConstructor
    open GodelianExplorer.Client.Components.Basic
    open Bolero
    open System
    open System.Collections.Generic

    type Items() =
        let items =
            seq {
                for i in 0..999999 do
                    yield i
            }

        interface IReadOnlyCollection<int> with
            member this.Count = 1000000

        interface IEnumerable<int> with
            member this.GetEnumerator() = items.GetEnumerator()

            member this.GetEnumerator() =
                items.GetEnumerator() :> Collections.IEnumerator

    let items = Items()

    type Term =
        | Value of bigint
        | Add of Term * Term
        | Mul of Term * Term

    let rec toNode (term: Term) : Bolero.Node =
        match term with
        | Value b -> text (b.ToString())
        | Add(l, r) ->
            concat {
                toNode l
                text " + "
                toNode r
            }
        | Mul(l, r) ->
            concat {
                toNode l
                text " * "
                toNode r
            }

    // Godelian constructor for terms
    let pick = createGodelianConstructorFor<Term> ()

    let counterPage model dispatch =
        concat {
            h1 {
                attr.``class`` "title"
                "Term Explorer"
            }

            div {
                div {
                    table {
                        attr.``class`` "bordered full-width"

                        thead {
                            tr {
                                th { 
                                    // Width should be just enough to show the index
                                    attr.style $"width: 50px;"
                                    "Index" 
                                
                                }
                                th { "Term" }
                                th { "Pair" }
                                th { "Ordered Pair" }
                                th { "Set" }
                                th { "Unordered List" }
                                th { "Ordered List" }
                            }
                        }

                        tbody {

                            virtualize.comp {
                                virtualize.itemSize 50.0f // Height of each item in pixels
                                let! i = virtualize.items items
                                let i = bigint i
                                let term = pick i
                                let pair = i |> Mappings.nat2pair |> sprintf "%A"
                                let orderedPair = i |> Mappings.nat2orderedpair |> sprintf "%A"
                                let setI = i |> Mappings.nat2set |> sprintf "%A"
                                let listI = i |> Mappings.nat2unorderedlist |> sprintf "%A"
                                let orderedListI = i |> Mappings.nat2orderedlist |> sprintf "%A"

                                tr {
                                    attr.style $"height: 2em;"
                                    td { string i }
                                    td { toNode term }
                                    td { string pair }
                                    td { string orderedPair }
                                    td { string setI }
                                    td { string listI }
                                    td { string orderedListI }
                                }
                            }
                        }
                    }
                }
            }
        }

    type CounterComponent() =
        inherit ElmishComponent<CounterModel, CounterMessage>()

        override this.ShouldRender(oldModel, newModel) = oldModel <> newModel

        override this.View model dispatch = counterPage model dispatch

    let mapCommands (dispatch: Model.Messages.Message -> unit) (m: CounterMessage) : unit = dispatch (CounterMsg m)

    let inline counter (model: Model) (dispatch: Model.Messages.Message -> unit) : Node =
        ecomp<CounterComponent, _, _> model.counter (mapCommands dispatch) { attr.empty () }
