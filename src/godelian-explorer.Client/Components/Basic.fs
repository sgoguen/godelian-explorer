module GodelianExplorer.Client.Components.Basic


open Bolero
open Bolero.Html

type InputModel = { label: string; value: string }

type Input() =
    inherit ElmishComponent<InputModel, string>()

    // Check for model changes by only looking at the value.
    override this.ShouldRender(oldModel, newModel) =
        oldModel.value <> newModel.value

    override this.View model dispatch =
        label {
            model.label
            input {
                attr.value model.value
                on.change (fun e -> dispatch (unbox e.Value))
            }
        }

let inline inputHandler label value handler =
            ecomp<Input,_,_>
                { label = label; value = value }
                (fun n -> handler(n))
                { attr.empty() }