﻿open NUnit.Framework
open Addition

[<TestFixture>]
type Additiontests() =    
    
    [<Test>]
    member this.APosBPosSimpleTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[0] <- 1
        array1.[9999] <- 1

        array2.[0] <- 1
        array2.[9999] <- 1

        array3.[0] <- 1
        array3.[9999] <- 2
        
        Assert.AreEqual(array3, (Addition.Main array1 array2 [|0|]))

    [<Test>]
    member this.APosBPos1JumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[0] <- 1
        array1.[9999] <- 9

        array2.[0] <- 1
        array2.[9999] <- 1

        array3.[0] <- 1
        array3.[9998] <- 1
        
        Assert.AreEqual(array3, (Addition.Main array1 array2 [|0|]))

    [<Test>]
    member this.APosBPosMore1JumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[0] <- 1
        for i in 2 .. 9999 do
            array1.[i] <- 9

        array2.[0] <- 1
        array2.[9999] <- 1

        array3.[0] <- 1
        array3.[1] <- 1
        Assert.AreEqual(array3, (Addition.Main array1 array2 [|0|]))

    [<Test>]
    member this.APosBPosNotFirstJumpTestNotOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)

        array1.[0] <- 1
        array1.[9999] <- 1
        array1.[9998] <- 9

        array2.[0] <- 1
        array2.[9998] <- 1

        array3.[0] <- 1
        array3.[9999] <- 1
        array3.[9997] <- 1
        
        Assert.AreEqual(array3, (Addition.Main array1 array2 [|0|]))

    [<Test>]
    member this.APosBPosMore1JumpTestOverflow() =
        let array1 = Array.zeroCreate(10000)
        let array2 = Array.zeroCreate(10000)
        let array3  = Array.zeroCreate(10000)
        let flag = [|0|]

        array1.[0] <- 1
        for i in 1 .. 9999 do
            array1.[i] <- 9

        array2.[0] <- 1
        array2.[9999] <- 1

        array3.[0] <- 1
        let array4 = Addition.Main array1 array2 flag
        for i in 1 .. 9999 do
            if array4.[i] = 10
            then printfn "%A" i

        Assert.AreEqual([|1|], flag)
        Assert.AreEqual(array3, array4)