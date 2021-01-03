namespace BenchmarkerGUI

open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media
open Elmish
open Avalonia.FuncUI.Elmish
open HardwareInformation

module SystemInformation =
    type State =
        {
            machineInformation: MachineInformation option
        }
    let init =
        {
            machineInformation = None
        }
    
    let update (msg: Msg) (state: State) : State =
        match msg with
        | UpdateMachineInformation machineInformation -> { state with machineInformation = Some machineInformation }
        | Reset -> init
        | _ -> state
        
    let private createTextBlock (text: string, row: int, column: int) : IView<TextBlock> =
        TextBlock.create [
            TextBlock.foreground Brushes.White
            TextBlock.margin (left = 0.0, top = 0.0, right = 0.0, bottom = 5.0)
            Grid.row row
            Grid.column column
            TextBlock.text text
        ]
        
    let view (state: State) (dispatch: Msg -> unit) =
        match state.machineInformation with
            | None ->
                Grid.create [
                    Grid.margin 10.0
                    Grid.rowSpan 6
                    Grid.columnSpan 2
                    Grid.rowDefinitions "Auto, Auto, Auto, Auto, Auto, Auto"
                    Grid.columnDefinitions "100,400"
                    Grid.children [
                        TextBlock.create [
                            Grid.row 3
                            Grid.column 2
                            TextBlock.text "Loading.."
                        ]
                    ]
                ]
            | Some machineInformation -> 
                let mutable ramInformation = ""
                let ramSticks = machineInformation.RAMSticks |> ResizeArray
                
                if ramSticks |> Seq.forall (fun ram -> ram.Capacity = ramSticks.[0].Capacity) then
                    ramInformation <- $"{ramSticks.[0].CapacityHRF} @ {ramSticks.[0].Speed} MHz x {ramSticks.Count} Sticks"
                else
                    ramInformation <- ramSticks |> Seq.map (fun ram -> $"{ram.CapacityHRF} @ {ram.Speed} MHz") |> String.concat ", "
                |> ignore
                
                Grid.create [
                    Grid.margin 10.0
                    Grid.rowSpan 6
                    Grid.columnSpan 2
                    Grid.rowDefinitions "Auto, Auto, Auto, Auto, Auto, Auto"
                    Grid.columnDefinitions "100,400"
                    Grid.children [
                        createTextBlock("OS", 0, 0)
                        createTextBlock($"{machineInformation.Platform}, {machineInformation.Cpu.Architecture}, {machineInformation.OperatingSystem}", 0, 1)
                        createTextBlock("Processor", 1, 0)
                        createTextBlock(machineInformation.Cpu.Name, 1, 1)
                        createTextBlock("Cores", 2, 0)
                        createTextBlock(($"{machineInformation.Cpu.PhysicalCores} Cores, {machineInformation.Cpu.LogicalCores} Threads @ {(float machineInformation.Cpu.NormalClockSpeed / 1000.0)} GHz"), 2, 1)
                        createTextBlock("RAM", 3, 0)
                        createTextBlock((ramInformation), 3, 1)
                        createTextBlock("Mainboard", 4, 0)
                        createTextBlock($"{machineInformation.SmBios.BoardName} {machineInformation.SmBios.BoardVersion} by {machineInformation.SmBios.BoardVendor}", 4, 1)
                        createTextBlock("BIOS", 5, 0)
                        createTextBlock($"{machineInformation.SmBios.BIOSCodename} {machineInformation.SmBios.BIOSVersion} by {machineInformation.SmBios.BIOSVendor}", 5, 1)
                    ]
                ]

