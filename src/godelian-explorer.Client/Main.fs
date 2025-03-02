module godelian_explorer.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero

open Model
open Messages

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override _.CssScope = CssScopes.MyApp

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        let update = update this.HttpClient

        Program.mkProgram (fun _ -> initModel, Cmd.none) update Views.Application.view
        |> Program.withRouter router
