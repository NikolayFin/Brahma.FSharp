

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let Main arg =
    
    let array1 = Array.zeroCreate(10000)
    let array2 = Array.zeroCreate(10000)
    let array3 = Array.zeroCreate(10000)
    let flag = [|0|]
    let operation = [|1|]

    array1.[9999] <- 1
    array2.[9999] <- 1
    array3.[9999] <- 1

    let platformName = "*"
    
    let localWorkSize = 100
    let deviceType = DeviceType.Default

    let provider =
        try  ComputeProvider.Create(platformName, deviceType)
        with 
        | ex -> failwith ex.Message

    let commandQueue = new CommandQueue(provider, provider.Devices |> Seq.head)    

    let command = 
        <@
            fun (rng:_1D) (a:array<_>) (b:array<_>) (c:array<_>) (fl:array<_>) (operation:array<_>)->
                let r = rng.GlobalID0
                let counter = ref r
                let count = ref true
                
                if operation.[0] = 1 
                then 
                    c.[r] <- a.[r] + b.[r]
                
                    while !counter >= 0 && !count do
                        count := false
                        if !counter > 0 && c.[!counter] > 9 
                        then
                            count := true
                            c.[!counter] <!+ (-1 * 10)
                            c.[!counter - 1] <! c.[!counter - 1] + 1
                        elif !counter = 0 && c.[!counter] > 9
                        then
                            c.[!counter] <!+ (-1 * 10)
                            fl.[0] <! 1
                        counter := !counter - 1

                if operation.[0] = -1
                then 
                    c.[r] <- c.[r] + a.[r] - b.[r]

                    while !counter >= 0 && !count do
                        count := false
                        if !counter > 0 && c.[!counter] < 0 
                        then
                            count := true
                            c.[!counter] <!+ (10)
                            c.[!counter - 1] <! c.[!counter - 1] - 1
                        elif !counter = 0 && c.[!counter] < 0
                        then
                            c.[!counter] <!+ 10
                            fl.[0] <! -1
                        counter := !counter - 1
                
                
        @>

    let kernel, kernelPrepare, kernelRun = provider.Compile command
    
    let d = (new _1D(array1.Length, localWorkSize))
    
    kernelPrepare d array1 array2 array3 flag operation
    
    //kernelRun()//.Finish()
    
    let rec fib n =
        if n = 5
        then
            kernelPrepare d array2 array3 array1 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
            kernelPrepare d array3 array1 array2 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
            kernelPrepare d array1 array2 array3 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
        elif n = 4
        then
            kernelPrepare d array3 array1 array2 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
            kernelPrepare d array1 array2 array3 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
        elif n = 3
        then
            kernelPrepare d array1 array2 array3 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
        else
            fib (n - 3)
            kernelPrepare d array2 array3 array1 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
            kernelPrepare d array3 array1 array2 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
            kernelPrepare d array1 array2 array3 flag operation
            commandQueue.Add(kernelRun()).Finish()
            ()
    fib arg
    //let _ = commandQueue.Add(kernelRun()).Finish()
    let _= commandQueue.Add(array3.ToHost provider)
    let _= commandQueue.Add(flag.ToHost provider)

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    
    
Main 5000 // возвращение целочисленного кода выхода
