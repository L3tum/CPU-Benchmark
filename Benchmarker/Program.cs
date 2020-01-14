#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking;
using Benchmarking.Results;
using CommandLine;
using CPU_Benchmark_Common;
using HardwareInformation;
using ShellProgressBar;

#endregion

namespace Benchmarker
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var options = new Options();

#if RELEASE
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
			options = new Options { Benchmark = "avx2int", Threads = 1, Runs = 1 };
#endif

			if (!OptionParser.ParseOptions(options))
			{
				Console.ReadLine();

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

			if (options.Stress)
			{
				var cancellationTokenSource = new CancellationTokenSource();
				var consoleTask = Task.Run(() =>
				{
					Thread.CurrentThread.Priority = ThreadPriority.Highest;
					Console.WriteLine("Press any key to stop the stress test.");
					Console.ReadKey(true);

					cancellationTokenSource.Cancel();
				});

				var stressTester = new StressTestRunner(options);
				stressTester.Prepare();
				var completed = stressTester.RunStressTest(cancellationTokenSource.Token);

				Task.WaitAll(consoleTask);

				Console.WriteLine("You've completed {0} benchmark iteration{1}; ~{2} per thread.", completed,
					completed == 1 ? "" : "s",
					Math.Round(completed / (double) options.Threads, 2));

				return;
			}

			Console.WriteLine("Starting Benchmark...");

			Console.WriteLine();

			ResultSaver.Init(options);

			var runner = new BenchmarkRunner(options, information);

			runner.Prepare();

			var poptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				BackgroundColor = ConsoleColor.DarkYellow,
				ProgressCharacter = '─',
				ProgressBarOnBottom = true,
				CollapseWhenFinished = false
			};

			using (var pbar = new ProgressBar((int) BenchmarkRunner.TotalOverall,
				$"Running Benchmark {options.Benchmark} on {options.Threads} threads {options.Runs} times", poptions))
			{
				using var ct = new CancellationTokenSource();
				var t = Task.Run(() =>
				{
					Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

					while (!ct.IsCancellationRequested)
					{
						lock (BenchmarkRunner.CurrentRunningBenchmark)
						{
							pbar.Tick(BenchmarkRunner.FinishedOverall,
								$"Overall. Currently running {BenchmarkRunner.CurrentRunningBenchmark} on {options.Threads} threads {options.Runs} times");
						}

						Thread.Sleep(200);
					}
				}, ct.Token);

				try
				{
					runner.RunBenchmark();
				}
				catch (ArgumentException e)
				{
					Console.WriteLine(e.Message);

					return;
				}

				pbar.Tick((int) BenchmarkRunner.TotalOverall);

				ct.Cancel();
				pbar.Tick((int) BenchmarkRunner.TotalOverall);
				t.GetAwaiter().GetResult();
			}

			Console.WriteLine();

			Console.WriteLine(
				Util.FormatResults(new Dictionary<uint, List<Result>> {{options.Threads, runner.Results}}));

			Console.ReadLine();
		}
	}
}