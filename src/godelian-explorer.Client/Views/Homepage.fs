namespace Views

module Homepage =


    open Bolero
    open Bolero.Html

    open Model
    open Model.Messages

    open Bolero.Html


    let homePage model dispatch =
        div {
            attr.``class`` "content"
            h1 { attr.``class`` "title"; "Welcome to Bolero!" }
            p { "This application demonstrates Bolero's major features." }
            ul {
                li {
                    "The entire application is driven by "
                    a {
                        attr.target "_blank"
                        attr.href "https://fsbolero.github.io/docs/Elmish"
                        "Elmish"
                    }
                    "."
                }
                li {
                    "The menu on the left switches pages based on "
                    a {
                        attr.target "_blank"
                        attr.href "https://fsbolero.github.io/docs/Routing"
                        "routes"
                    }
                    "."
                }
                li {
                    "The "
                    a { router.HRef Counter; "Counter" }
                    " page demonstrates event handlers and data binding in "
                    a {
                        attr.target "_blank"
                        attr.href "https://fsbolero.github.io/docs/Templating"
                        "HTML templates"
                    }
                    "."
                }
                li {
                    "The "
                    a { router.HRef Data; "Download data" }
                    " page demonstrates the use of HTTP requests to the server."
                }
                p { "Enjoy writing awesome apps!" }
            }
        }