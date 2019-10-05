#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking;
using Benchmarking.Results;
using CommandLine;
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
						options.Threads = Environment.ProcessorCount;
					}
					else
					{
						options.Threads = 1;
					}
				}
			});

			if (options.ListBenchmarks)
			{
				Console.WriteLine(string.Join(Environment.NewLine, BenchmarkRunner.GetAvailableBenchmarks()));

				Console.ReadLine();

				return;
			}

			if (options.ListResults)
			{
				ResultSaver.Init();
				Console.WriteLine();
				Console.WriteLine(FormatResults(ResultSaver.GetResults()));

				Console.ReadLine();

				return;
			}

			if (options?.Benchmark == null)
			{
				Console.ReadLine();

				return;
			}

			if (!BenchmarkRunner.GetAvailableBenchmarks()
				.Any(a => a.ToLower().Equals(options.Benchmark)))
			{
				Console.WriteLine("Benchmark name not recognized!");
				Console.ReadLine();

				return;
			}
#else
			options = new Options {Benchmark = "encryption", Threads = 1, Runs = 1};
#endif
			Console.WriteLine("Gathing hardware information...");

			var information = MachineInformationGatherer.GatherInformation();

			Console.WriteLine("OS:             {0}", information.OperatingSystem);
			Console.WriteLine("Processor:      {0}", information.Cpu.Name);
			Console.WriteLine("Architecture:   {0}", information.Cpu.Architecture);
			Console.WriteLine("Logical Cores:  {0}", information.Cpu.LogicalCores);
			Console.WriteLine("Physical Cores: {0}", information.Cpu.PhysicalCores);

			Console.WriteLine();

			Console.WriteLine("Starting Benchmark...");

			Console.WriteLine();

			ResultSaver.Init();

			var runner = new BenchmarkRunner(options);

			runner.Prepare();

			var poptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				BackgroundColor = ConsoleColor.DarkYellow,
				ProgressCharacter = '─',
				ProgressBarOnBottom = true,
				CollapseWhenFinished = false
			};
			var childOptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Green,
				BackgroundColor = ConsoleColor.DarkGreen,
				ProgressCharacter = '─',
				ProgressBarOnBottom = true
				//CollapseWhenFinished = false
			};

			using (var pbar = new ProgressBar(BenchmarkRunner.TotalOverall,
				$"Running Benchmark {options.Benchmark} on {options.Threads} threads {options.Runs} times", poptions))
			{
				var ct = new CancellationTokenSource();
				var t = Task.Run(() =>
				{
					ChildProgressBar childProgressBar = null;
					var lastBenchmark = string.Empty;

					while (!ct.IsCancellationRequested)
					{
						lock (BenchmarkRunner.CurrentRunningBenchmark)
						{
							pbar.Tick(BenchmarkRunner.FinishedOverall,
								$"Overall. Currently running {BenchmarkRunner.CurrentRunningBenchmark} on {options.Threads} threads {options.Runs} times");

#if FALSE == TRUE
							if (childProgressBar == null &&
							    !string.IsNullOrEmpty(BenchmarkRunner.CurrentRunningBenchmark) && !string.Equals(
								    BenchmarkRunner.CurrentRunningBenchmark,
								    options.Benchmark,
								    StringComparison.CurrentCultureIgnoreCase))
							{
								childProgressBar = pbar.Spawn(BenchmarkRunner.SingleBenchmarkTotal,
									BenchmarkRunner.CurrentRunningBenchmark,
									childOptions);
								lastBenchmark = BenchmarkRunner.CurrentRunningBenchmark;
							}

							if (childProgressBar != null)
							{
								if (lastBenchmark != BenchmarkRunner.CurrentRunningBenchmark)
								{
									childProgressBar.Tick(BenchmarkRunner.SingleBenchmarkTotal);
									childProgressBar.Dispose();

									childProgressBar = pbar.Spawn(BenchmarkRunner.SingleBenchmarkTotal,
										BenchmarkRunner.CurrentRunningBenchmark,
										childOptions);
									lastBenchmark = BenchmarkRunner.CurrentRunningBenchmark;
								}

								childProgressBar.Tick(BenchmarkRunner.CurrentBenchmarkFinished);
							}
#endif
						}

						Thread.Sleep(200);
					}

					childProgressBar?.Tick(BenchmarkRunner.SingleBenchmarkTotal);
					childProgressBar?.Dispose();
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

				pbar.Tick(100);

				ct.Cancel();
				t.GetAwaiter().GetResult();
			}

			Console.WriteLine();

			Console.WriteLine(FormatResults(runner.Results));

			foreach (var runnerResult in runner.Results)
			{
				ResultSaver.SaveResult(runnerResult);
			}

			Console.WriteLine();
			Console.WriteLine("Uploading results...");

			if (ResultSaver.UploadResults().Result)
			{
				Console.WriteLine("Done!");
			}
			else
			{
				Console.WriteLine("Failed uploading results!");
			}

			Console.ReadLine();
		}

		private static string FormatResults(List<Result> results)
		{
			return results.ToStringTable(
				new[] {"Benchmark", "Time", "Reference (3900x)", "Points", "Reference(3900x)"},
				r => r.Benchmark,
				r => FormatTime(r.Timing),
				r => FormatTime(r.ReferenceTiming),
				r => r.Points,
				r => r.ReferencePoints);
		}

		private static string FormatTime(double time)
		{
			var ts = TimeSpan.FromMilliseconds(time);

			var parts = $"{ts.Days:D2}d:{ts.Hours:D2}h:{ts.Minutes:D2}m:{ts.Seconds:D2}s:{ts.Milliseconds:D3}ms"
				.Split(':')
				.SkipWhile(s => Regex.Match(s, @"^00\w").Success) // skip zero-valued components
				.ToArray();
			return string.Join(" ", parts); // combine the result
		}
	}
}