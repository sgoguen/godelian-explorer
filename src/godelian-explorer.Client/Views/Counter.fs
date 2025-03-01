namespace Views

module Counter =


    open Bolero.Html

    open Model
    open Model.Messages

    open GodelianToolkit
    open GodelianToolkit.AutoConstructor

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
                    on.click (fun _ -> dispatch Decrement)
                    attr.``class`` "button"
                    "-"
                }

                input {
                    attr.``type`` "number"
                    attr.id "counter"
                    attr.``class`` "input"
                    bind.input.int model.counter (fun v -> dispatch (SetCounter v))
                }

                button {
                    on.click (fun _ -> dispatch Increment)
                    attr.``class`` "button"
                    "+"
                }
            }

            div {
                let term = pick model.counter

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
