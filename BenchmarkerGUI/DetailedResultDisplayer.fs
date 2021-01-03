namespace BenchmarkerGUI

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media
open Benchmarking.Results

module DetailedResultDisplayer =
    type State =
        {
            save: Save
            resultType: ResultType
            mouseX: float
            mouseY: float
        }
        
    let init =
        {
            save = null
            resultType = ResultType.MultiThreaded
            mouseX = 0.0
            mouseY = 0.0
        }
        
    let update (msg: Msg) (state: State) : State =
        match msg with
        | UpdateMousePosition point ->
            {state with mouseX = point.X; mouseY = point.Y}
        | UpdateDisplayedResultType resultType ->
            {state with resultType = resultType}
        | UpdateDisplayedSave save ->
            {state with save = save}
        | _ -> state
        
    let private getDetailedResults (save: Save) (resultType: ResultType): IView list =
        let results = if resultType = ResultType.MultiThreaded then save.MultiThreadedResults else save.SingleThreadedResults
        let mutable views = List.empty
        
        for result in results do
            views <- TextBlock.create [
                TextBlock.text $"{result.Benchmark}: {result.Iterations}"
                TextBlock.margin 10.0
                DockPanel.dock Dock.Left
            ] :: views
            
        views |> Seq.cast<IView> |> Seq.toList
        
    let view (state: State) (dispatch: Msg -> unit) =
        Border.create [
            Border.top state.mouseX
            Border.left state.mouseY
            Border.width 400.0
            Border.height 200.0
            Border.borderBrush Brushes.White
            Border.borderThickness 2.0
            Border.child (
                WrapPanel.create [
                    WrapPanel.children (getDetailedResults state.save state.resultType)
                ]
            )
        ]

