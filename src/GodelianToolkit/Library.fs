namespace GodelianToolkit



open GodelianTooklit

// Define a simple Lambda Calculus language
// with a simple default syntax
type Term =
    | Var of int // Variable reference
    | Lambda of Term // Abstraction - (Lambas)
    | App of Term * Term // Function application

type Person =
    { Name: string
      Age: int
      Friends: Person list }

open AutoConstructor
open IndentingPrinter



module Example1 =

    let naiveConstructor: bigint -> Term =
        combineChoices
            [ fun enc n -> Var(int (n))
              fun enc (Pair(name, body)) -> Lambda(enc body)
              fun enc (Pair(l, r)) -> App(enc l, enc r) ]

//for i in 0I..10I do
//   printfn "%A - %A" i (naiveConstructor i)

//  We get this output
//0 - Var 0
//1 - Lambda (Var 0)
//2 - App (Var 0, Var 0)
//3 - Var 1
//4 - Lambda (Lambda (Var 0))
//5 - App (Var 0, Lambda (Var 0))
//6 - Var 2
//7 - Lambda (Lambda (Var 0))
//8 - App (Lambda (Var 0), Lambda (Var 0))
//9 - Var 3
//10 - Lambda (Var 0)

module Example2 =

    type Term =
        | Value of bool
        | Add of Term * Term
        | Mul of Term * Term

    //  Now, let's see what we can do to implement it for this...
    let pick = createGodelianConstructorFor<Term> ()


// for i in 0I..10I do
//   printfn "%A - %A" i (pick i)


module Example3 =
    open System

    type expression =
        | Variable of int (* a variable *)
        | Numeral of int (* integer constant *)
        | Plus of expression * expression (* addition [e1 + e2] *)
    //| Minus of expression * expression (* difference [e1 - e2] *)
    //| Times of expression * expression (* product [e1 * e2] *)
    //| Divide of expression * expression (* quotient [e1 / e2] *)
    //| Remainder of expression * expression (* remainder [e1 % e2] *)

    type boolean =
        | True (* constant [true] *)
        | False (* constant [false] *)
        | Equal of expression * expression (* equal [e1 = e2] *)
        | Less of expression * expression (* less than [e1 < e2] *)
        | And of boolean * boolean (* conjunction [b1 and b2] *)
        | Or of boolean * boolean (* disjunction [b1 or b2] *)
        | Not of boolean (* negation [not b] *)

    type command =
        //| Skip (* no operation [skip] *)
        //| New of string * expression * command (* variable declaration [new x := e in c] *)
        //| Print of expression (* print expression [print e] *)
        | Assign of string * expression (* assign a variable [x := e] *)
        | Sequence of command * command (* sequence commands [c1 ; c2] *)
        | While of boolean * command (* loop [while b do c done] *)
        | Conditional of boolean * command * command (* conditional [if b then c1 else c2 end] *)


    let rec printCommand (p: IndentingPrinter.Printer) (c: command) : unit =
        //  Let's print the above commands to resemble that of a language that
        //  resembles JavaScript where we don't have to declare variables before we use them
        match c with
        | Assign(var, expr) ->
            p.Print(var)
            p.Print(" = ")
            printExpression expr p
            p.Print(";")
        | Sequence(c1, c2) ->
            printCommand p c1
            p.PrintNewLine()
            printCommand p c2
        | While(cond, body) ->
            p.Print("while (")
            printBoolean cond p
            p.Print(") {")
            p.PushIndentation()
            p.PrintNewLine()
            printCommand p body
            p.PopIndentation()
            p.PrintNewLine()
            p.Print("}")
        | Conditional(cond, thenCmd, elseCmd) ->
            p.Print("if (")
            printBoolean cond p
            p.Print(") {")
            p.PushIndentation()
            p.PrintNewLine()
            printCommand p thenCmd
            p.PopIndentation()
            p.PrintNewLine()
            p.Print("} else {")
            p.PushIndentation()
            p.PrintNewLine()
            printCommand p elseCmd
            p.PopIndentation()
            p.PrintNewLine()
            p.Print("}")

    and printExpression (e: expression) (p: Printer) : unit =
        match e with
        | Variable v -> p.Print(sprintf "x%d" v)
        | Numeral n -> p.Print(sprintf "%d" n)
        | Plus(e1, e2) ->
            printExpression e1 p
            p.Print(" + ")
            printExpression e2 p

    and printBoolean (b: boolean) (p: Printer) : unit =
        match b with
        | True -> p.Print("true")
        | False -> p.Print("false")
        | Equal(e1, e2) ->
            printExpression e1 p
            p.Print(" == ")
            printExpression e2 p
        | Less(e1, e2) ->
            printExpression e1 p
            p.Print(" < ")
            printExpression e2 p
        | And(b1, b2) ->
            printBoolean b1 p
            p.Print(" && ")
            printBoolean b2 p
        | Or(b1, b2) ->
            printBoolean b1 p
            p.Print(" || ")
            printBoolean b2 p
        | Not(b) ->
            p.Print("!")
            printBoolean b p




    //  Now, let's see what we can do to implement it for this...
    let pick = createGodelianConstructorFor<command> ()



// pick 12398723498732423409824232498723498734I |> printCommand printer

// printer.Flush()

//  Prints the following
//
//  while (x10 == x0 + x0 + x0 + x0 || false || true && false || true && !false) {
//    if (478 == 18 + x0) {
//        if (true) {
//            a = x0;
//        } else {
//            a = x0;
//        }
//        a = x0;
//        a = x0;
//        if (false) {
//            a = x0;
//            a = x0;
//        } else {
//            a = x0;
//        }
//    } else {
//        if (true) {
//            a = x0;
//            a = x0;
//        } else {
//            a = x0;
//        }
//        e = 1;
//    }
//  }
