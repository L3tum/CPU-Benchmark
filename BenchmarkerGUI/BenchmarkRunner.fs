namespace BenchmarkerGUI

open System
open System.Reactive.Disposables
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.Media
open Benchmarking
open Benchmarking
open Benchmarking.Results
open Elmish
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

module BenchmarkRunner =   
    type Logger(dispatch: Msg -> unit) =
        let dispatchFunc = dispatch
        
        interface ILogger with
            member this.BeginScope(state) = Disposable.Empty
            member this.IsEnabled(logLevel) = true
            member this.Log(logLevel, eventId, state, ``exception``, formatter) =
                let mutable message =
                    if formatter <> null then
                        formatter.Invoke(state, ``exception``)
                    else
                        ""
                message <- $"{logLevel}: {message}"
                message |> UpdateLog |> dispatch
    
    type State =
        {
            options: Options
            logger: ILogger
            logs: string list
        }
        
    let init =
        let options = Options()
        options.Benchmark <- "all"
        options.WarmupTime <- 1000
        
        {
            options = options
            logger = null
            logs = List.Empty
        }
        
    let logger (state) =
        let sub (dispatch: Msg -> unit) =
            Logger(dispatch) :> ILogger |> UpdateLogger |> dispatch
        Cmd.ofSub sub
        
    let update (msg: Msg) (state: State) =
        match msg with
        | UpdateBenchmark benchmark ->
            let options = state.options
            options.Benchmark <- benchmark
            {state with options = options}, Cmd.none
        | StartBenchmark mode ->
            let options = state.options
            options.BenchmarkingMode <- mode
            
            let thread = async {
                let runner = Runner(options, state.logger)
                runner.RunBenchmarks()
                
                return UpdateResults((runner.Results |> ResizeArray |> Seq.toList))
            }
            {state with options = options; logs = List.empty}, Cmd.OfAsync.result thread
        | UpdateLogger logger ->
            { state with logger = logger; logs = List.empty }, Cmd.none
        | UpdateLog logEntry ->
            let logs = logEntry :: state.logs
            {state with logs = logs}, Cmd.none
        | Reset -> init, Cmd.none
        | _ -> state, Cmd.none
        
    let view (state: State) (dispatch: Msg -> unit) =
        DockPanel.create [
            DockPanel.margin 5.0
            DockPanel.dock Dock.Top
            DockPanel.children [
                TextBlock.create [
                    TextBlock.textWrapping TextWrapping.Wrap
                    TextBlock.text (state.logs |> List.rev |> String.concat "\n")
                    TextBlock.verticalScrollBarVisibility ScrollBarVisibility.Auto
                ]
            ]
        ]
