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

            h1 {
                attr.``class`` "title"
                "Welcome to the Godelian Explorer!"
            }

            p {
                """
                This is a simple web application that demonstrates the use of the Godelian Toolkit. 
                """
            }

            h2 { "What is the Godelian Toolkit?" }

            p {
                """
                The Godelian Toolkit makes complex data structures easy to enumerate and index.  It does this by
                creating a bijective mapping between the data structure and the natural numbers.  This allows you to
                easily generate and data structures, almost as if you had an infinite database of them.
                """
            }
        }
