namespace Views

module Counter =


    open Bolero.Html

    open Model
    open Model.Messages


    let counterPage model dispatch =
        concat {
            h1 { attr.``class`` "title"; "A simple counter" }
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
        }