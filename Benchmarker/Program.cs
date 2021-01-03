#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Benchmarking;
using Benchmarking.Results;
using Benchmarking.Util;
using HardwareInformation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using ShellProgressBar;

#endregion

namespace Benchmarker
{
    internal class Program
    {
        private static readonly ILoggerFactory LoggerFactory =
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { builder.AddConsole(); });

        private static readonly ILogger<Runner> Logger = LoggerFactory.CreateLogger<Runner>();

        private static Options? GetOptions(string[] args)
        {
            var arguments = new Arguments();

#if (RELEASE && FALSE)
			Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
			{
				options = opts;

				if (opts == null)
				{
					return;
				}

				if (opts.Threads == 0)
				{
					if (opts.Multithreaded)
					{
						options.Threads = (uint) Environment.ProcessorCount;
					}
					else
					{
						options.Threads = 1;
					}
				}
			});

			if (args.Contains("--help") || args.Contains("-h"))
			{
				return;
			}

			if (options.ClearSave)
			{
				ResultSaver.Clear();

				Console.WriteLine("Successfully cleared all saved data!");
				Console.ReadLine();

				return;
			}

#else
            arguments = new Arguments {Benchmark = "parsing"};
#endif

            return OptionParser.ParseOptions(arguments);
        }

        private static void Main(string[] args)
        {
            var saver = new ResultSaver();
            AppDomain.CurrentDomain.ProcessExit +=
                (sender, eventArgs) => CurrentDomainOnProcessExit(sender, eventArgs, saver);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
            
            var options = GetOptions(args);

            if (options is null)
            {
                return;
            }

            Console.WriteLine("Gathering hardware information...");
            var information = MachineInformationGatherer.GatherInformation();
            Console.WriteLine("OS:             {0}", information.OperatingSystem);
            Console.WriteLine("Processor:      {0}", information.Cpu.Name);
            Console.WriteLine("Architecture:   {0}", information.Cpu.Architecture);
            Console.WriteLine("Logical Cores:  {0}", information.Cpu.LogicalCores);
            Console.WriteLine("Physical Cores: {0}", information.Cpu.PhysicalCores);
            Console.WriteLine();
            Console.WriteLine("Starting Benchmark...");
            Console.WriteLine();
            
            var runner = new Runner(options, new NullLogger<Runner>());

            Console.WriteLine("Running the following benchmarks in approx. {1}: {0}",
                string.Join(", ", runner.GetBenchmarksToRun().Select(benchmark => benchmark.GetName())),
                Helper.FormatTime(runner.GetTotalTime()));
            Console.WriteLine();
            Console.WriteLine();

            using var ct = new CancellationTokenSource();
            var t = new Thread(() => DisplayProgressbar(ref runner, ref options, ct.Token));
            t.Start();

            runner.RunBenchmarks();
            ct.Cancel();
            t.Join();

            saver.CreateOrUpdateSaveForCurrentRun(information, runner.Results);
            var save = saver.GetSave("current");
            if (save is null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine(
                Util.FormatResults(new Dictionary<int, List<Result>>
                {
                    {1, save.SingleThreadedResults},
                    {Environment.ProcessorCount, save.MultiThreadedResults}
                }));
            Console.WriteLine();
        }

        private static void CurrentDomainOnProcessExit(object? sender, EventArgs e, ResultSaver saver)
        {
            saver.WriteSaves();
        }

        private static void DisplayProgressbar(ref Runner runner, ref Options options, CancellationToken ct)
        {
            if (!options.EnableProgressBar)
            {
                return;
            }
            
            while (string.IsNullOrEmpty(runner.CurrentBenchmark))
            {
                Thread.Sleep(100);
            }

            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            var totalTime = (int) runner.GetTotalTime();
            var poptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.DarkGray,
                ProgressCharacter = '#',
                ProgressBarOnBottom = true,
                CollapseWhenFinished = false
            };
            using var pbar = new ProgressBar(totalTime,
                $"Running Benchmark {options.Benchmark} on {options.Threads} threads {options.Runs} times",
                poptions);
            var sw = Stopwatch.StartNew();

            while (!ct.IsCancellationRequested)
            {
                pbar.Tick((int) sw.ElapsedMilliseconds,
                    TimeSpan.FromMilliseconds(totalTime - sw.ElapsedMilliseconds),
                    $"Overall. Currently running {runner.CurrentBenchmark} on {options.Threads} threads {options.Runs} times");

                Thread.Sleep(200);
            }

            pbar.Tick(totalTime);
        }
    }
}