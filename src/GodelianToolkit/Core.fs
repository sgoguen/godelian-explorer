namespace GodelianToolkit

////////////////////////////////////////////////////////////////////////
///  The Gödelian Toolkit
////////////////////////////////////////////////////////////////////////

module Nat =
    type Nat = bigint

    let sqrt (z: bigint) : bigint =
        if z < 0I then
            invalidArg "z" "Cannot compute the square root of a negative number"
        elif z = 0I then
            0I
        else
            let rec newtonRaphson (x: bigint) : bigint =
                let nextX = (x + z / x) / 2I
                if nextX >= x then x else newtonRaphson nextX

            newtonRaphson z

module MonoPairing =
    open Nat

    let pair (a: Nat) (b: Nat) =
        if a < b then b * b + a else a * a + a + b

    let unpair (n: Nat) =
        let s = sqrt n
        if n - s * s < s then (n - s * s, s) else (s, n - s * s - s)

module Mappings =
    open Nat

    let nat2pair (z: bigint) : bigint * bigint = MonoPairing.unpair z

    let nat2orderedpair (z: bigint) : bigint * bigint =
        let (a, b) = nat2pair z
        (a, a + b)

    let pair2ordered (a: bigint, b: bigint) = (a, a + b + 1I)

    let nat2pairWithId (skip: bigint) (z: bigint) : bigint * bigint =
        if z < skip then
            (z, z)
        else
            let (a, b) = nat2pair (z - skip)
            (a + skip, b + skip)

    let skip (i: bigint) (n: bigint) : bigint = n + i

    let nat2orderedPairWithId (z: bigint) : bigint * bigint =
        let (a, b) = nat2pair z |> pair2ordered
        (a + 1I, b + 1I)


    let nat2set (n: Nat) : Set<Nat> =
        let rec nat2exps n x =
            if n = 0I then
                []
            else
                let xs = nat2exps (n / 2I) (x + 1I)
                if n % 2I = 0I then xs else x :: xs

        (if n >= 0I then nat2exps n 0I else []) |> Set.ofList

    let nat2orderedlist (n: Nat) =
        let s = nat2set n |> Set.toSeq
        [ for (i, a) in Seq.indexed s -> a - (bigint i) ]

    let rec nat2unorderedlist (n: Nat) =
        if n = 0I then
            []
        else
            let (b, a) = nat2pair n
            a :: nat2unorderedlist b

module GodelianTooklit =

    let memoizeRec f =
        let cache = System.Collections.Concurrent.ConcurrentDictionary()
        let rec recF x = cache.GetOrAdd(x, lazy f recF x).Value
        recF


    // A constructor for an infinite domain
    //type InfCtor<'T> = InfCtor of ctor: (bigint -> 'T)
    type InfCtor<'T> = (bigint -> 'T)

    type FinCtor<'T> = FindCtor of size: bigint * ctor: (bigint -> 'T)

    let sqrt (z: bigint) : bigint =
        if z < 0I then
            invalidArg "z" "Cannot compute the square root of a negative number"
        elif z = 0I then
            0I
        else
            let rec newtonRaphson (x: bigint) : bigint =
                let nextX = (x + z / x) / 2I
                if nextX >= x then x else newtonRaphson nextX

            newtonRaphson z

    let encodePair (z: bigint) : bigint * bigint =
        let m = sqrt (z)
        let m2 = m * m
        if z - m2 < m then (z - m2, m) else (m, m2 + 2I * m - z)

    let (|Pair|) = encodePair

    let decodePair (p: bigint * bigint) : bigint =
        let (x, y) = p
        let m = max x y
        m * m + m + x - y


    let combineChoices (functionList: ((InfCtor<'a>) -> InfCtor<'a>) list) (n: bigint) : 'a =
        let length = bigint (List.length functionList)

        let rec chooseFunction n =
            let (d, r) = bigint.DivRem(n, length)
            let f = functionList.[int (r)]
            f chooseFunction d

        chooseFunction n


    let combineChoicesWithContext
        (getOptions: 'a -> (('a -> InfCtor<'b>) -> InfCtor<'b>) list)
        (initialContext: 'a)
        : InfCtor<'b> =

        //  We add a parameter that now includes context
        let rec chooseFunction context n =
            let functionList = getOptions (context)
            let length = bigint (List.length functionList)
            let (d, r) = bigint.DivRem(n, length)
            let f = functionList.[int (r)]
            f chooseFunction d

        chooseFunction initialContext


    let tryFiniteFirst
        (numberOfFiniteOptions: int)
        (finiteConstuctor: int -> 'b)
        (infiniteConstructors: ('a -> InfCtor<'b>) list)
        =
        let numberOfFiniteOptions = numberOfFiniteOptions - 1
        let length = bigint (List.length infiniteConstructors)

        if numberOfFiniteOptions >= 0 then
            [ (fun enc n ->
                  if n <= (bigint numberOfFiniteOptions) then
                      finiteConstuctor (int n)
                  else
                      let n = n - (bigint (numberOfFiniteOptions + 1))
                      let (d, r) = bigint.DivRem(n, length)
                      let f = infiniteConstructors.[int r]
                      f enc d) ]
        else
            infiniteConstructors

    module Counter =
        let makeCounter () =
            let mutable counter = 0I

            fun () ->
                let result = counter
                counter <- counter + 1I
                result


    module Strings =
        module Alpha =
            let rec fromInt n =
                if n < 26I then
                    string (char (int 'a' + int n))
                else
                    fromInt (n / 26I) + string (char (int 'a' + int (n % 26I)))
