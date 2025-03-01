namespace GodelianToolkit

module AutoConstructor =

    open System
    open FSharp.Reflection
    open GodelianTooklit



    /// Given an integer and a count, decode the integer into a list of ‘count’ integers
    let rec decodeParts (n: bigint) (k: int) : bigint list =
        if k <= 0 then
            []
        elif k = 1 then
            [ n ]
        else
            let (a, b) = encodePair n
            a :: decodeParts b (k - 1)

    //  Convert a bigint to a variable name using letters
    //  a, b, ..., aa, ab, ..., ba, bb, ...
    let toVarName (n: bigint) : string =
        let rec toVarName' n acc =
            if n < 0I then
                acc
            else
                let charCode = int (n % 26I) + int 'a'
                let newChar = char charCode
                toVarName' (n / 26I - 1I) (string newChar + acc)

        toVarName' n ""

    /// Recursively build a Gödelian constructor for any supported type.
    /// The returned function takes a bigint and produces an instance boxed as obj.
    let createGodelianConstructorForType (recMake: Type -> InfCtor<'obj>) (t: Type) : (InfCtor<'obj>) =
        if t = typeof<string> then
            fun n -> box (toVarName (n))
        else if t = typeof<int> then
            fun n -> box (int n)
        elif t = typeof<bool> then
            // For booleans, we might use even/odd as false/true.
            fun n -> box ((n % 2I) = 0I)
        elif t = typeof<bigint> then
            fun n -> box n
        elif FSharpType.IsUnion(t) then
            // For each union case, create a constructor function that uses the Gödelian
            // encoding to “fill in” its fields.
            let caseConstructors =
                lazy
                    // For union types, enumerate the union cases.
                    let cases = FSharpType.GetUnionCases(t)

                    cases
                    |> Array.map (fun unionCase ->
                        let fields = unionCase.GetFields()
                        let fieldConstructors = fields |> Array.map (fun f -> recMake f.PropertyType)

                        fun (n: bigint) ->
                            if fieldConstructors.Length = 0 then
                                // Nullary case: no fields.
                                FSharpValue.MakeUnion(unionCase, [||])
                            elif fieldConstructors.Length = 1 then
                                // Single field: pass the entire number.
                                let fieldVal = fieldConstructors.[0] n
                                FSharpValue.MakeUnion(unionCase, [| fieldVal |])
                            else
                                // Multiple fields: split n into as many parts as needed.
                                let parts = decodeParts n fieldConstructors.Length
                                let fieldVals = fieldConstructors |> Array.mapi (fun i cons -> cons parts.[i])
                                FSharpValue.MakeUnion(unionCase, fieldVals))

            fun (n: bigint) ->
                let caseConstructors = caseConstructors.Value
                // Use the input number to choose the union case.
                let len = bigint (Array.length caseConstructors)
                let (d, r) = bigint.DivRem(n, len)
                // d is passed into the chosen case’s constructor for its field(s)
                caseConstructors.[int r] d
        elif FSharpType.IsRecord(t) then
            // For records, build a constructor that decodes each field.

            let fieldConstructors =
                lazy
                    let fields = FSharpType.GetRecordFields(t)
                    fields |> Array.map (fun f -> recMake f.PropertyType)

            fun (n: bigint) ->
                let fieldConstructors = fieldConstructors.Value

                if fieldConstructors.Length = 0 then
                    FSharpValue.MakeRecord(t, [||])
                elif fieldConstructors.Length = 1 then
                    let fieldVal = fieldConstructors.[0] n
                    FSharpValue.MakeRecord(t, [| fieldVal |])
                else
                    let parts = decodeParts n fieldConstructors.Length
                    let fieldVals = fieldConstructors |> Array.mapi (fun i cons -> cons parts.[i])
                    FSharpValue.MakeRecord(t, fieldVals)
        else
            failwithf "Type %A is not supported by the Godelian constructor" t



    let makeConstructor = memoizeRec createGodelianConstructorForType

    /// The public API: automatically create a Gödelian constructor for type 'T.
    let createGodelianConstructorFor<'T> () : bigint -> 'T =
        let cons = makeConstructor (typeof<'T>)
        fun n -> cons n |> unbox<'T>