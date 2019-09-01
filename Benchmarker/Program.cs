#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking;
using CommandLine;

#endregion

namespace Benchmarker
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var options = new Options();

#if RELEASE
			Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(opts =>
			{
				options = opts;

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
#else
			options = new Options {Benchmark = "ARITHMETIC_INT", Threads = Environment.ProcessorCount, Runs = 1};
#endif
			var runner = new BenchmarkRunner(options);

			Console.WriteLine(
				$"Running Benchmark {options.Benchmark} on {options.Threads} threads {options.Runs} times");

			using (var progress = new ProgressBar())
			{
				var ct = new CancellationTokenSource();
				var t = Task.Run(() =>
				{
					while (!ct.IsCancellationRequested)
					{
						progress.Report(BenchmarkRunner.CurrentProgress);

						Thread.Sleep(20);
					}
				}, ct.Token);

				runner.RunBenchmark();

				progress.Report(1.0d);

				ct.Cancel();
				t.GetAwaiter().GetResult();
			}

			Console.WriteLine();

			Console.WriteLine(runner.Results.ToStringTable(new []{"Benchmark", "Time", "Reference (3900x)", "Points", "Reference(3900x)"},
				r => r.Benchmark,
				r => FormatTime(r.Timing),
				r => FormatTime(r.ReferenceTiming),
				r => r.Points,
				r => r.ReferencePoints));

			Console.ReadLine();
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