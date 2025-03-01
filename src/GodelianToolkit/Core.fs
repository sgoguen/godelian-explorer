namespace GodelianToolkit

////////////////////////////////////////////////////////////////////////
///  The Gödelian Toolkit
////////////////////////////////////////////////////////////////////////

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