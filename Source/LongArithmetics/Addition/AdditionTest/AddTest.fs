open NUnit.Framework
open Addition

[<TestFixture>]
type Additiontests() =    
    
    [<Test>]
    member this.AddiionSimpleTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[9999] <- 1

        array2.[9999] <- 1

        array3.[9999] <- 2
        
        Assert.AreEqual(array3, (Addition.Main array1 [|1|] array2 [|0|]))

    [<Test>]
    member this.Addiion1JumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[9999] <- 9

        array2.[9999] <- 1

        array3.[9998] <- 1
        
        Assert.AreEqual(array3, (Addition.Main array1 [|1|] array2 [|0|]))

    [<Test>]
    member this.AddiionMore1JumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        for i in 1 .. 9999 do
            array1.[i] <- 9

        array2.[9999] <- 1

        array3.[0] <- 1
        Assert.AreEqual(array3, (Addition.Main array1 [|1|] array2 [|0|]))

    [<Test>]
    member this.AddiionNotFirstJumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[9999] <- 1
        array1.[9998] <- 9

        array2.[9998] <- 1

        array3.[9999] <- 1
        array3.[9997] <- 1
        
        Assert.AreEqual(array3, (Addition.Main array1 [|1|] array2 [|0|]))

    [<Test>]
    member this.AddiionMore1JumpTestOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)
        let flag = [|0|]

        for i in 0 .. 9999 do
            array1.[i] <- 9

        array2.[9999] <- 1

        let array4 = Addition.Main array1 [|1|] array2 flag
        
        for i in 1 .. 9999 do
            array3.[i] <- 0

        for i in 1 .. 9999 do
            if array4.[i] = 10
            then printfn "%A" i

       // Assert.AreEqual([|1|], flag)
        Assert.AreEqual(array3, array4)

    
    [<Test>]
    member this.SubtractionSimpleTest() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[9999] <- 1
        array1.[9998] <- 9

        array2.[9999] <- 1

        array3.[9998] <- 9

        let array4 = Addition.Main array1 [|-1|] array2 [|0|]
        
        Assert.AreEqual(array3, array4)

    [<Test>]
    member this.Subtraction1Jump() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[9999] <- 1
        array1.[9998] <- 9

        array2.[9999] <- 9

        array3.[9999] <- 2
        array3.[9998] <- 8
        
        Assert.AreEqual(array3, (Addition.Main array1 [|-1|] array2 [|0|]))

    [<Test>]
    member this.SubtractionMore1Jump() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[0] <- 9
        for i in 1 .. 9999 do
            array1.[i] <- 1

        for i in 1 .. 9999 do
            array2.[i] <- 9

        array3.[9999] <- 2
        for i in 1 .. 9998 do
            array3.[i] <- 1
        array3.[0] <- 8
        let array4 = (Addition.Main array1 [|-1|] array2 [|0|])
        for i in 0 .. 9999 do
            if array4.[i] = 2
            then printfn "%A" i
        Assert.AreEqual(array3, array4)