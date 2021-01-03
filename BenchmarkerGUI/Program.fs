namespace BenchmarkerGUI

open System.Diagnostics
open Avalonia.Controls
open BenchmarkerGUI.Main
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
        
type App() =
    inherit Application()
    
    override this.Initialize() =
        this.Styles.Add(FluentTheme(baseUri=null, Mode=FluentThemeMode.Dark))
        
    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()


module Program =
    [<EntryPoint>]
    let main(args: string[]) =
        Process.GetCurrentProcess().PriorityClass <- ProcessPriorityClass.BelowNormal
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args)