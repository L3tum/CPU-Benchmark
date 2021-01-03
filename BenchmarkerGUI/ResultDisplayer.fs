namespace BenchmarkerGUI

open System
open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Media
open Avalonia.VisualTree
open Benchmarking.Results
open Elmish
open HardwareInformation
open HardwareInformation.Information.Cpu

module ResultDisplayer =
    type State =
        {
            saver: ResultSaver
            resultType: ResultType
            saves: Save list
            savesToDisplay: Save list
            machineInformation: MachineInformation option
            maxScore: uint64
        }
        
    let saveSaves (state: State) (saveCurrent: bool) =
        state.saver.WriteSaves(saveCurrent)
        
    let private getResultByResultType (save: Save) (resultType: ResultType) : uint64 =
        match resultType with
        | ResultType.MultiThreaded -> save.OverallMultiThreaded
        | ResultType.SingleThreaded -> save.OverallSingleThreaded
        
    let private getFilteredSaves (saves: Save list) (resultType: ResultType) : Save list =
        saves |> List.filter (fun save -> (getResultByResultType save resultType) > 1uL) |> List.sortByDescending (fun save -> (getResultByResultType save resultType)) |> List.truncate 10
        
    let private getMaxScore (saves: Save list) (resultType: ResultType) : uint64 =
        if saves.Length > 0 then getResultByResultType (saves |> Seq.maxBy (fun save -> getResultByResultType save resultType)) resultType else 0uL
        
    let init =
        let saver = ResultSaver()
        let saves = saver.GetListOfSaves() |> ResizeArray |> Seq.map (saver.GetSave) |> Seq.toList
        let filteredSaves = getFilteredSaves saves ResultType.MultiThreaded
        let maxScore = getMaxScore filteredSaves ResultType.MultiThreaded
        
        {
            saver = saver
            resultType = ResultType.MultiThreaded
            saves = saves
            savesToDisplay = filteredSaves
            machineInformation = None
            maxScore = maxScore
        }
    
    let update (msg: Msg) (state: State) =       
        match msg with
        | UpdateDisplayedResultType resultType ->
            let filteredSaves = getFilteredSaves state.saves resultType
            let maxScore = getMaxScore filteredSaves resultType
            { state with resultType = resultType; savesToDisplay = filteredSaves; maxScore = maxScore }, Cmd.none
        | UpdateResults results ->
            let machineInformation = match state.machineInformation with
                                        // Force gathering of information if it still isn't available, as it should be now since running the benchmarks forces it as well
                                        | None -> MachineInformationGatherer.GatherInformation()
                                        | Some machineInformation -> machineInformation
            let saver = state.saver
            try
                saver.CreateOrUpdateSaveForCurrentRun (machineInformation, results)
            with ex -> Console.WriteLine(ex.Message)
            
            let save = saver.GetSave("current")
            let saves = if state.saves.Head.Created = save.Created then (state.saves |> List.map (fun sav -> if sav.Created = save.Created then save else sav)) else save :: state.saves
            let filteredSaves = getFilteredSaves saves state.resultType
            let maxScore = getMaxScore filteredSaves state.resultType
            
            { state with saver = saver; saves = saves; savesToDisplay = filteredSaves; maxScore = maxScore }, Cmd.ofMsg(UpdateCurrentSave(save))
        | UpdateMachineInformation machineInformation -> {state with machineInformation = Some machineInformation}, Cmd.none
        | Reset -> init, Cmd.none
        | _ -> state, Cmd.none
        
    let private saveTemplate (save: Save) (index: int) (resultType: ResultType) (cpuName: string) (maxScore: uint64) (isCurrentSave: bool) (dispatch: Msg -> unit) =
        let score = getResultByResultType save resultType
        let maxScore = if maxScore > 0uL then maxScore else score
        let color = if isCurrentSave then
                        Colors.DarkOrange
                    elif cpuName = save.MachineInformation.Cpu.Name then
                        Color(byte 255, byte 196, byte 108, byte 0)
                    elif save.MachineInformation.Cpu.Vendor = Vendors.Intel then
                        Colors.AliceBlue
                    else Colors.DarkRed

        let gradient = LinearGradientBrush()
        gradient.StartPoint <- RelativePoint(0.0, 0.0, RelativeUnit.Relative)
        gradient.EndPoint <- RelativePoint((double score / double maxScore), (double score / double maxScore), RelativeUnit.Relative)
        gradient.GradientStops.Add(GradientStop(color, (double score / double maxScore)))
        gradient.GradientStops.Add(GradientStop(Colors.Black, (double score / double maxScore)))
        
        DockPanel.create [
            DockPanel.background gradient
            DockPanel.dock Dock.Top
            DockPanel.margin (left = 0.0, top = 0.0, right = 0.0, bottom = 2.0)
            DockPanel.onPointerEnter (fun args ->
                let mutable source: IVisual = (args.Source :?> IVisual)
                let mutable position = Point()
                
                while position.X = 0.0 && position.Y = 0.0 && source <> null do
                    position <- source.Bounds.BottomRight
                    source <- source.VisualParent
                
                position |> UpdateMousePosition |> dispatch
                save |> UpdateDisplayedSave |> dispatch
            )
            DockPanel.onPointerLeave (fun args -> null |> UpdateDisplayedSave |> dispatch)
            DockPanel.children [
                Grid.create [
                    Grid.margin 10.0
                    Grid.columnDefinitions "400,50"
                    Grid.rowDefinitions "Auto"
                    Grid.rowSpan 1
                    Grid.columnSpan 2
                    Grid.horizontalAlignment HorizontalAlignment.Stretch
                    Grid.children [
                        TextBlock.create [
                            Grid.column 0
                            TextBlock.margin (left = 0.0, top = 0.0, right = 5.0, bottom = 0.0)
                            TextBlock.foreground "#ffffff"
                            TextBlock.fontSize 12.0
                            TextBlock.text $"{index + 1}. {save.MachineInformation.Cpu.PhysicalCores}C/{save.MachineInformation.Cpu.LogicalCores}T @ {float save.MachineInformation.Cpu.NormalClockSpeed / 1000.0} GHz, {save.MachineInformation.Cpu.Name}"
                        ]
                        TextBlock.create [
                            Grid.column 1
                            TextBlock.foreground "#ffffff"
                            TextBlock.fontSize 12.0
                            TextBlock.text (score.ToString())
                        ]
                    ]
                ]
            ]
        ]

    let private saveListView (selectedIndex: int) (state: State) (dispatch: Msg -> unit) =
        let cpuName = match state.machineInformation with
                        | None -> ""
                        | Some machineInformation -> machineInformation.Cpu.Name
        
        let mutable currentSave = state.saver.GetSave("current")
        
        if currentSave = null then
            currentSave <- Save() 
        
        let saveViews = state.savesToDisplay |> List.mapi (fun index save -> (saveTemplate save index state.resultType cpuName state.maxScore (currentSave.Created = save.Created) dispatch)) |> Seq.cast<IView> |> Seq.toList
        
        DockPanel.create [
            DockPanel.dock Dock.Bottom
            DockPanel.lastChildFill false
            DockPanel.children saveViews
        ]
        
    let view (state: State) (dispatch: Msg -> unit) =
        DockPanel.create [
            DockPanel.margin 10.0
            DockPanel.children [
                ComboBox.create [
                    DockPanel.dock Dock.Top
                    ComboBox.horizontalAlignment HorizontalAlignment.Center
                    ComboBox.onSelectedIndexChanged (fun index ->
                        if index = 0 then ResultType.MultiThreaded |> UpdateDisplayedResultType |> dispatch
                        elif index = 1 then ResultType.SingleThreaded |> UpdateDisplayedResultType |> dispatch
                    )
                    ComboBox.viewItems [
                        ComboBoxItem.create [
                            ComboBoxItem.content "Multi Threaded"
                        ]
                        ComboBoxItem.create [
                            ComboBoxItem.content "Single Threaded"
                        ]
                    ]
                    ComboBox.selectedIndex (if state.resultType = ResultType.MultiThreaded then 0 else 1)
                ]
                Separator.create [
                    DockPanel.dock Dock.Top
                    Separator.minHeight 10.0
                ]
                saveListView -1 state dispatch
            ]
        ]
