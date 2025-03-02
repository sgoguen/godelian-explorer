namespace Views

module Counter =


    open Bolero.Html

    open Model
    open Model.Messages

    open Model.Counter

    open GodelianToolkit
    open GodelianToolkit.AutoConstructor
    open GodelianExplorer.Client.Components.Basic
    open Bolero

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

    //  Now, let's see what we can do to implement it for this...
    let pick = createGodelianConstructorFor<Term> ()

    let counterPage model dispatch =
        concat {
            h1 {
                attr.``class`` "title"
                "A simple counter!!!!"
            }

            p {
                button {
                    on.click (fun _ -> dispatch Decr)
                    attr.``class`` "button"
                    "-"
                }

                input {
                    attr.``type`` "number"
                    attr.id "counter"
                    attr.``class`` "input"
                    bind.input.int model.count (fun v -> dispatch (SetCount v))
                }

                button {
                    on.click (fun _ -> dispatch Incr)
                    attr.``class`` "button"
                    "+"
                }
            }

            div {
                let term = pick (bigint model.count)

                h2 {
                    attr.``class`` "subtitle"
                    "Term"
                }

                div {
                    attr.``class`` "box"
                    toNode term
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
