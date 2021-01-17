namespace BenchmarkerGUI

open System
open System.Diagnostics
open System.Runtime.ExceptionServices
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
    let handler(obj: Object) (args: FirstChanceExceptionEventArgs) =
        Console.WriteLine(args.Exception.Message)
        Console.WriteLine(args.Exception.StackTrace)
    
    [<EntryPoint>]
    let main(args: string[]) =
        Process.GetCurrentProcess().PriorityClass <- ProcessPriorityClass.BelowNormal
        AppDomain.CurrentDomain.FirstChanceException.AddHandler(EventHandler<FirstChanceExceptionEventArgs>(handler))
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args)