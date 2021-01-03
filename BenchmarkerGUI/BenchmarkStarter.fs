namespace BenchmarkerGUI

open System
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Benchmarking
open Benchmarking.Results
open Elmish

module BenchmarkStarter =
    type State =
        {
            runningMultiThreaded: bool
            runningSingleThreaded: bool
            currentSave: Save option
        }
        
    let init =
        {
            runningMultiThreaded = false
            runningSingleThreaded = false
            currentSave = None
        }
        
    let update (msg: Msg) (state: State) : State * Cmd<Msg> =
        match msg with
        | StartMultiThreaded -> {state with runningMultiThreaded = true}, Cmd.ofMsg (StartBenchmark(Options.Mode.MULTI_THREADED))
        | StartSingleThreaded -> {state with runningSingleThreaded = true}, Cmd.ofMsg (StartBenchmark(Options.Mode.SINGLE_THREADED))
        | UpdateCurrentSave save -> {runningMultiThreaded = false; runningSingleThreaded = false; currentSave = Some save}, Cmd.none
        | Reset -> init, Cmd.none
        | _ -> state, Cmd.none
        
    let view (state: State) (dispatch: Msg -> unit) =
        let singleThreadedResult = if state.currentSave.IsSome && state.currentSave.Value.OverallSingleThreaded > 0uL then state.currentSave.Value.OverallSingleThreaded.ToString() else "---"
        let multiThreadedResult = if state.currentSave.IsSome && state.currentSave.Value.OverallMultiThreaded > 0uL then state.currentSave.Value.OverallMultiThreaded.ToString() else "---"
        let mpRatio = if state.currentSave.IsSome && state.currentSave.Value.OverallSingleThreaded > 0uL && state.currentSave.Value.OverallMultiThreaded > 0uL then Math.Round(double state.currentSave.Value.OverallMultiThreaded / double state.currentSave.Value.OverallSingleThreaded, 2).ToString() else "---"
        
        Grid.create [
            Grid.margin 10.0
            Grid.rowSpan 5
            Grid.columnSpan 3
            Grid.rowDefinitions "Auto, 10, Auto, 10, Auto"
            Grid.columnDefinitions "300, 100, 100"
            Grid.children [
                TextBlock.create [
                    Grid.row 0
                    Grid.column 0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text "CPU (MultiThreaded)"
                ]
                TextBlock.create [
                    Grid.row 0
                    Grid.column 1
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text multiThreadedResult
                ]
                Button.create [
                    Grid.row 0
                    Grid.column 2
                    Button.verticalAlignment VerticalAlignment.Center
                    Button.content (if state.runningMultiThreaded then "Running" else "Start")
                    Button.isEnabled (not (state.runningSingleThreaded || state.runningMultiThreaded))
                    Button.onClick (fun args -> StartMultiThreaded |> dispatch)
                ]
                TextBlock.create [
                    Grid.row 2
                    Grid.column 0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text "CPU (SingleThreaded)"
                ]
                TextBlock.create [
                    Grid.row 2
                    Grid.column 1
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text singleThreadedResult
                ]
                Button.create [
                    Grid.row 2
                    Grid.column 2
                    Button.verticalAlignment VerticalAlignment.Center
                    Button.content (if state.runningSingleThreaded then "Running" else "Start")
                    Button.isEnabled (not (state.runningSingleThreaded || state.runningMultiThreaded))
                    Button.onClick (fun args -> StartSingleThreaded |> dispatch)
                ]
                TextBlock.create [
                    Grid.row 4
                    Grid.column 0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text "MP Ratio"
                ]
                TextBlock.create [
                    Grid.row 4
                    Grid.column 1
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text singleThreadedResult
                ]
                TextBlock.create [
                    Grid.row 4
                    Grid.column 2
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.text mpRatio
                ]
            ]
        ]

