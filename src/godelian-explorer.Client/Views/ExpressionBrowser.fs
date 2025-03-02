namespace Views



module ExpressionBrowser =
    open Bolero
    open Bolero.Html
    open Elmish
    open Microsoft.JSInterop
    open System
    open Model
    open Model.Messages
    open GodelianToolkit
    open GodelianToolkit.AutoConstructor

    // Reusing the Term type from Counter.fs
    type Term =
        | Value of bigint
        | Add of Term * Term
        | Mul of Term * Term

    // let rec toNode (term: Term) : Bolero.Node =
    //     match term with
    //     | Value b -> text (b.ToString())
    //     | Add(l, r) ->
    //         concat {
    //             toNode l
    //             text " + "
    //             toNode r
    //         }
    //     | Mul(l, r) ->
    //         concat {
    //             toNode l
    //             text " * "
    //             toNode r
    //         }

    // // The number of expressions to load at once
    // let pageSize = 20
    
    // // Message types for the expression browser
    // type BrowserMsg =
    //     | LoadMore
    //     | SetStartIndex of int

    // // State for the expression browser
    // type BrowserState = {
    //     StartIndex: int
    //     EndIndex: int
    //     Expressions: Map<int, Term>
    // }

    // // Initialize browser state
    // let init() = 
    //     let pick = createGodelianConstructorFor<Term>()
    //     let expressions = 
    //         [0..pageSize] 
    //         |> List.map (fun i -> i, pick i)
    //         |> Map.ofList
        
    //     {
    //         StartIndex = 0
    //         EndIndex = pageSize
    //         Expressions = expressions
    //     }

    // // Update function for browser messages
    // let update (jsRuntime: IJSRuntime) msg state =
    //     match msg with
    //     | LoadMore ->
    //         let pick = createGodelianConstructorFor<Term>()
    //         let newEndIndex = state.EndIndex + pageSize
    //         let newExpressions = 
    //             [(state.EndIndex + 1)..newEndIndex]
    //             |> List.map (fun i -> i, pick i)
    //             |> Map.ofList
            
    //         { state with 
    //             EndIndex = newEndIndex
    //             Expressions = Map.fold (fun acc k v -> Map.add k v acc) state.Expressions newExpressions 
    //         }
    //     | SetStartIndex index ->
    //         { state with StartIndex = index }
    
    // // View function for the browser
    // let view (state: BrowserState) (dispatch: BrowserMsg -> unit) =
    //     let handleScroll (e: Browser.Types.UIEvent) =
    //         let element = e.target :?> Browser.Types.HTMLElement
    //         let scrollTop = element.scrollTop
    //         let scrollHeight = element.scrollHeight
    //         let clientHeight = element.clientHeight

    //         // Load more when scrolled to 80% of the way down
    //         if scrollTop > (scrollHeight - clientHeight) * 0.8 then
    //             dispatch LoadMore

    //     div {
    //         attr.``class`` "expression-browser"
    //         attr.style "height: 80vh; overflow-y: scroll; padding: 1em;"
    //         on.scroll handleScroll

    //         h1 {
    //             attr.``class`` "title"
    //             "Expression Browser"
    //         }

    //         p {
    //             "Showing expressions from "
    //             text (string state.StartIndex)
    //             " to "
    //             text (string state.EndIndex)
    //         }

    //         div {
    //             attr.``class`` "expressions-container"
                
    //             for i in state.StartIndex..state.EndIndex do
    //                 match Map.tryFind i state.Expressions with
    //                 | Some term ->
    //                     div {
    //                         attr.``class`` "box expression-item"
    //                         attr.style "margin-bottom: 1em;"
                            
    //                         div {
    //                             attr.``class`` "expression-index"
    //                             attr.style "font-weight: bold; margin-bottom: 0.5em;"
    //                             "Expression #" + (string i)
    //                         }
                            
    //                         div {
    //                             attr.``class`` "expression-content"
    //                             toNode term
    //                         }
    //                     }
    //                 | None -> 
    //                     div { 
    //                         attr.``class`` "box is-loading"
    //                         "Loading..." 
    //                     }
    //         }
            
    //         // Loading indicator at the bottom
    //         div {
    //             attr.``class`` "has-text-centered"
    //             attr.style "padding: 1em;"
    //             "Loading more expressions..."
    //         }
    //     }