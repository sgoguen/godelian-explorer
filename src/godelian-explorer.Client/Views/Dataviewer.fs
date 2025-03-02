namespace Views




module Common =

    open Bolero
    open Bolero.Html

    open Model
    open Model.Messages

    let menuItem (model: Model) (page: Page) (itemText: string) =
        li {
            a {
                attr.``class`` (if model.page = page then "is-active" else "")
                router.HRef page
                itemText
            }
        }

    let errorNotification errorText closeCallback =
        div {
            attr.``class`` "notification is-warning"

            cond closeCallback
            <| function
                | None -> empty ()
                | Some closeCallback ->
                    button {
                        attr.``class`` "delete"
                        on.click closeCallback
                    }

            text errorText
        }


module Application =

    open Bolero
    open Bolero.Html

    open Model
    open Model.Messages

    open Common
    open Views.Homepage
    open Views.Counter

    let view model dispatch =
        div {
            attr.``class`` "columns"

            // The Side Menu
            aside {
                attr.``class`` "column sidebar is-narrow"

                section {
                    attr.``class`` "section"

                    nav {
                        attr.``class`` "menu"

                        ul {
                            attr.``class`` "menu-list"
                            menuItem model Home "Home"
                            menuItem model Counter "Term Explorer"
                        }
                    }
                }
            }

            //  The Main Content Panel
            div {
                attr.``class`` "column"

                section {
                    attr.``class`` "section"

                    cond model.page
                    <| function
                        | Home -> homePage model dispatch
                        | Counter -> counter model dispatch

                    div {
                        attr.id "notification-area"

                        cond model.error
                        <| function
                            | None -> empty ()
                            | Some err -> errorNotification err (Some(fun _ -> dispatch ClearError))
                    }
                }
            }
        }
