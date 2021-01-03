namespace BenchmarkerGUI

open Avalonia
open Benchmarking
open Benchmarking.Results
open HardwareInformation
open Microsoft.Extensions.Logging

type ResultType =
    | SingleThreaded
    | MultiThreaded

type Msg =
    | UpdateBenchmark of string
    | StartBenchmark of Options.Mode
    | StartMultiThreaded
    | StartSingleThreaded
    | UpdateCurrentSave of Save
    | UpdateDisplayedResultType of ResultType
    | UpdateResults of Result list
    | UpdateMachineInformation of MachineInformation
    | UpdateLogger of ILogger
    | UpdateLog of string
    | DisplayPopup
    | UpdateMousePosition of Point
    | UpdateDisplayedSave of Save
    | Reset