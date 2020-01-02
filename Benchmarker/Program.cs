#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking;
using Benchmarking.Results;
using Benchmarking.Util;
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
			options = new Options { Benchmark = "zip", Threads = 1, Runs = 1 };
#endif

			if (options.ListBenchmarks)
			{
				Console.WriteLine(string.Join(Environment.NewLine, BenchmarkRunner.GetAvailableBenchmarks()));
				Console.ReadLine();

				return;
			}

			if (options.ListResults)
			{
				ResultSaver.Init(options);
				Console.WriteLine();
				Console.WriteLine(FormatResults(ResultSaver.GetResults()));
				Console.ReadLine();

				return;
			}

			if (options.Upload)
			{
				if (!ResultSaver.Init(options))
				{
					Console.WriteLine(
						"A restart or hardware change has been detected. Please redo the benchmark and upload it without restarting.");
					Console.ReadLine();

					return;
				}

				var result = ResultSaver.IsAllowedToUpload(options);

				if (result == ErrorCode.NOT_WINDOWS)
				{
					Console.WriteLine("You can only upload your results on a Windows machine.");
					Console.ReadLine();

					return;
				}

				if (result == ErrorCode.NO_CATEGORY_ALL)
				{
					Console.WriteLine("You can only upload your results after having completed all benchmarks.");
					Console.ReadLine();

					return;
				}

				Console.WriteLine();
				Console.WriteLine("Uploading results...");

				var response = ResultSaver.UploadResults().Result;

				if (response != null)
				{
					Console.WriteLine("Done!");
					Console.WriteLine("You can view your raw save here: {0}", response.RawPath);
					Console.WriteLine("You can view your parsed save here: {0}", response.WebsitePath);

					Console.WriteLine(
						"Click 'a' to open raw, 'b' to open parsed or any other key to close the program!");

					while (true)
					{
						var key = Console.ReadKey(true);

						if (key.Key == ConsoleKey.A)
						{
							Helper.OpenBrowser(response.RawPath);
						}
						else if (key.Key == ConsoleKey.B)
						{
							Helper.OpenBrowser(response.WebsitePath);
						}
						else
						{
							return;
						}
					}
				}

				Console.WriteLine("Failed uploading results!");

				Console.ReadLine();

				return;
			}

			if (options.Benchmark == null || string.IsNullOrWhiteSpace(options.Benchmark))
			{
				Console.WriteLine("Please specify a benchmark!");
				Console.ReadLine();

				return;
			}

			if (!BenchmarkRunner.GetAvailableBenchmarks()
				.Any(a => a.ToLowerInvariant().Equals(options.Benchmark.ToLowerInvariant())))
			{
				Console.WriteLine("Benchmark name not recognized!");
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

			Console.WriteLine(FormatResults(new Dictionary<uint, List<Result>> {{options.Threads, runner.Results}}));

			Console.ReadLine();
		}

		private static string FormatResults(Dictionary<uint, List<Result>> results)
		{
			var s = string.Empty;

			foreach (var keyValuePair in results)
			{
				s += $"Benchmarked on {keyValuePair.Key} Threads\n";

				s += keyValuePair.Value.ToStringTable(
					new[] {"Benchmark", "Time", "Reference", "Points", "Reference", "DataThroughput"},
					r => r.Benchmark,
					r => FormatTime(r.Timing),
					r => FormatTime(r.ReferenceTiming),
					r => r.Points,
					r => r.ReferencePoints,
					r => $"{Helper.FormatBytes((ulong) r.DataThroughput)}/s");
			}

			return s;
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