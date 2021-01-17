namespace BenchmarkerGUI

open System
open System.ComponentModel
open System.Threading
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media
open Benchmarking.Results
open HardwareInformation

module Main =
    open Avalonia.Controls
    open Avalonia.Layout
    open Avalonia.FuncUI.Elmish
    open Elmish
    
    let mutable hasAnsweredSave = false
    
    type State =
        {
            Benchmark: string
            BenchmarkStarterState: BenchmarkStarter.State
            ResultDisplayerState: ResultDisplayer.State
            BenchmarkRunnerState: BenchmarkRunner.State
            SystemInformationState: SystemInformation.State
            DetailedResultDisplayerState: DetailedResultDisplayer.State
            DisplayPopup: bool
            DisplayedDetailedSave: Save
        }
        
    let private exit (args: CancelEventArgs) (state: State) (dispatch: Msg -> unit) =
        if state.ResultDisplayerState.saver.GetSave("current") <> null then
            DisplayPopup |> dispatch
            args.Cancel <- not hasAnsweredSave
        
    let private exitHook (state: State) (mainWindow: HostWindow) =
        let sub (dispatch: Msg -> unit) =
            mainWindow.Closing.Add(fun args -> exit args state dispatch)

        Cmd.ofSub sub
        
    let init () =
        let machineInformationThread = async {
            return (MachineInformationGatherer.GatherInformation() |> UpdateMachineInformation)
        }
        
        {
            Benchmark = "all"
            BenchmarkStarterState = BenchmarkStarter.init
            ResultDisplayerState = ResultDisplayer.init
            BenchmarkRunnerState = BenchmarkRunner.init
            SystemInformationState = SystemInformation.init
            DetailedResultDisplayerState = DetailedResultDisplayer.init
            DisplayPopup = false
            DisplayedDetailedSave = null
        }, Cmd.OfAsync.result machineInformationThread
        
    let update (msg: Msg) (state: State) =
        let s, command = BenchmarkStarter.update msg state.BenchmarkStarterState
        let s2, command2 = ResultDisplayer.update msg state.ResultDisplayerState
        let s3, command3 = BenchmarkRunner.update msg state.BenchmarkRunnerState
        let s4 = SystemInformation.update msg state.SystemInformationState
        let s5 = DetailedResultDisplayer.update msg state.DetailedResultDisplayerState
        
        let newState, newCommand = match msg with
                                    | UpdateBenchmark benchmark -> { state with Benchmark = benchmark }, Cmd.none
                                    | DisplayPopup -> {state with DisplayPopup = true}, Cmd.none
                                    | UpdateDisplayedSave save -> {state with DisplayedDetailedSave = save}, Cmd.none
                                    | Reset -> init()
                                    | _ -> state, Cmd.none
        
        {
            newState with BenchmarkStarterState = s; ResultDisplayerState = s2; BenchmarkRunnerState = s3; SystemInformationState = s4; DetailedResultDisplayerState = s5
        }, Cmd.batch [newCommand; command; command2; command3]
        
    let private createPanel (title: string, inner: IView) : IView<Border> =
        Border.create[
            DockPanel.dock Dock.Top
            Border.borderBrush Brushes.Gray
            Border.borderThickness 1.0
            Border.margin (left = 0.0, right = 0.0, top = 0.0, bottom = 10.0)
            Border.child (
                DockPanel.create [
                    DockPanel.children [
                        TextBlock.create [
                            DockPanel.dock Dock.Top
                            TextBlock.text title
                            TextBlock.foreground Brushes.LightGray
                            TextBlock.fontSize 12.0
                            TextBlock.margin (left = 10.0, top = 5.0, right = 0.0, bottom = 5.0)
                        ]
                        inner
                    ]
                ]
            )
        ]
        
    let view (state: State) (dispatch: Msg -> unit) =
        Grid.create [
            Grid.children [
                DockPanel.create [
                    DockPanel.lastChildFill false
                    DockPanel.verticalAlignment VerticalAlignment.Stretch
                    DockPanel.horizontalAlignment HorizontalAlignment.Stretch
                    DockPanel.children [
                        DockPanel.create [
                            DockPanel.width 500.0
                            DockPanel.background Brushes.Black
                            DockPanel.dock Dock.Left
                            DockPanel.horizontalAlignment HorizontalAlignment.Left
                            DockPanel.verticalAlignment VerticalAlignment.Stretch
                            DockPanel.children [
                                createPanel ("Run Benchmark", BenchmarkStarter.view state.BenchmarkStarterState dispatch)
                                createPanel ("Your System", SystemInformation.view state.SystemInformationState dispatch)
                                createPanel ("Ranking", ResultDisplayer.view state.ResultDisplayerState dispatch)
                            ]
                        ]

                        BenchmarkRunner.view state.BenchmarkRunnerState dispatch
                    ]
                ]
                
                if state.DisplayedDetailedSave <> null then
                    DetailedResultDisplayer.view state.DetailedResultDisplayerState dispatch
                                        
                if state.DisplayPopup then
                    Border.create [
                        Border.background "#40000000"
                        Border.child (
                            DockPanel.create [
                                DockPanel.width 200.0
                                DockPanel.height 200.0
                                DockPanel.lastChildFill false
                                DockPanel.children [
                                    TextBlock.create [
                                        DockPanel.dock Dock.Top
                                        TextBlock.textWrapping TextWrapping.Wrap
                                        TextBlock.text "Do you want to save your current results?"
                                    ]
                                    DockPanel.create [
                                        DockPanel.dock Dock.Bottom
                                        DockPanel.lastChildFill false
                                        DockPanel.children [
                                            Button.create [
                                                DockPanel.dock Dock.Left
                                                Button.content (
                                                    TextBlock.create [
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                                                        TextBlock.text "Yes"
                                                    ]    
                                                )
                                                Button.width 50.0
                                                Button.onClick (fun args ->
                                                    hasAnsweredSave <- true
                                                    ResultDisplayer.saveSaves state.ResultDisplayerState true
            
                                                    (Application.Current.ApplicationLifetime :?> ClassicDesktopStyleApplicationLifetime).Shutdown()
                                                )
                                            ]
                                            
                                            Button.create [
                                                DockPanel.dock Dock.Right
                                                Button.content (
                                                    TextBlock.create [
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                                                        TextBlock.text "No"
                                                    ]    
                                                )
                                                Button.width 50.0
                                                Button.onClick (fun args ->
                                                    hasAnsweredSave <- true
                                                    
                                                    (Application.Current.ApplicationLifetime :?> ClassicDesktopStyleApplicationLifetime).Shutdown()
                                                )
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        )
                    ]
            ]
        ]
        
    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "CPU Benchmark"
            base.WindowState <- WindowState.Maximized
            this.SystemDecorations <- SystemDecorations.Full
                
            Program.mkProgram init update view
            |> Program.withHost this
            |> Program.withSubscription BenchmarkRunner.logger
            |> Program.withSubscription (fun state -> exitHook state this)
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.run

