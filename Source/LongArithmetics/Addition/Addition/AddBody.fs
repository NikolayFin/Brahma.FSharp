module Addition

open Brahma.Helpers
open OpenCL.Net
open Brahma.OpenCL
open Brahma.FSharp.OpenCL.Core
open Microsoft.FSharp.Quotations
open Brahma.FSharp.OpenCL.Extensions

let platformName = "*"
    
let localWorkSize = 1000
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
                        c.[!counter] <! c.[!counter] + 10
                        c.[!counter - 1] <! c.[!counter - 1] - 1
                            
                    elif !counter = 0 && c.[!counter] < 0
                    then
                        c.[!counter] <!+ 10
                        fl.[0] <! -1
                    counter := !counter - 1
                    barrier()
                
    @>

let kernel, kernelPrepare, kernelRun = provider.Compile command
    
let Main (array1:array<_>) (operation:array<_>) (array2:array<_>) (flag:array<_>) =
    
    let c = Array.zeroCreate(array1.Length)
    
    let d = (new _1D(array1.Length, localWorkSize))
    
    kernelPrepare d array1 array2 c flag operation
    
    let _ = commandQueue.Add(kernelRun()).Finish()
    let _ = commandQueue.Add(c.ToHost provider)
    let _ = commandQueue.Add(flag.ToHost provider)

    commandQueue.Dispose()
    provider.Dispose()
    provider.CloseAllBuffers()
    c
        //ignore (System.Console.Read())