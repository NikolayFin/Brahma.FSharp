module Addition

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main (array1:array<_>) (array2:array<_>) (flag:array<_>) =
    
    let c = Array.zeroCreate(array1.Length)

    let platformName = "*"
    
    let localWorkSize = 100
    let deviceType = DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let mutable commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)    

    let command = 
        <@
            fun (rng:_1D) (a:array<_>) (b:array<_>) (c:array<_>) (fl:array<_>)->
                let r = rng.GlobalID0
                let counter = ref r
                let count = ref true
                if a.[0] = b.[0]
                then
                    if r > 0
                    then
                        c.[r] <- a.[r] + b.[r]
                        //добавить цикл
                       (* while !counter > 0 && !count do
                            count := false
                            if !counter > 1 && c.[!counter] > 9 
                            then
                                count := true
                                c.[!counter] <!+ (-1 * 10)
                                c.[!counter - 1] <! c.[!counter - 1] + 1
                            counter := !counter - 1
                        
                        if c.[1] > 9
                        then 
                            fl.[0] <! 1
                            c.[1] <!+ -10*)
                    else c.[r] <- a.[r]
                //else тут должен быть блок обработки суммы чисел с разными знаками
                else 
                    if r > 0
                    then
                        c.[r] <- a.[r] - b.[r]
                      (*  while !counter > 0 && !count do
                            count := false
                            if !counter > 1 && c.[!counter] < 0
                            then
                                count := true
                                c.[!counter] <!+ 10
                                c.[!counter - 1] <! (c.[!counter - 1] - 1)
                            counter := !counter - 1*)
                    else c.[r] <- a.[r]

                if a.[0] = b.[0]
                then
                    while !counter > 1 && !count do
                            count := false
                            if !counter > 1 && c.[!counter] > 9 
                            then
                                count := true
                                c.[!counter] <!+ (-1 * 10)
                                c.[!counter - 1] <! c.[!counter - 1] + 1
                            counter := !counter - 1
                        
                    if c.[1] > 9
                    then 
                        fl.[0] <! 1
                        c.[1] <!+ -10
                else
                    while !counter > 0 && !count do
                        count := false
                        if !counter > 1 && c.[!counter] < 0
                        then
                            count := true
                            c.[!counter] <!+ 10
                            c.[!counter - 1] <! (c.[!counter - 1] - 1)
                    counter := !counter - 1
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    
    let d = (new _1D(array1.Length, localWorkSize))

    let rec digit (arr1:array<_>) (arr2:array<_>) i =
        if arr1.[i] < arr2.[i]
            then -1
            elif  arr1.[i] > arr2.[i]
                then 1
                elif i = arr1.Length - 1
                    then 0
                    else digit arr1 arr2 (i + 1)

    if digit array1 array2 1 = -1
        then
            kernelPrepare d array2 array1 c flag
        else
            kernelPrepare d array1 array2 c flag

    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(c.ToHost provider)
    let _ = commandQueue.Add(flag.ToHost provider)

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    c
    //ignore (System.Console.Read())