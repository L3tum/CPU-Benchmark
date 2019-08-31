#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Benchmarking;
using CommandLine;

#endregion

namespace Benchmarker
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Options options = new Options();

#if RELEASE
			Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(opts =>
			{
				options = opts;

				if (opts.Multithreaded && opts.Threads == 0)
				{
					options.Threads = Environment.ProcessorCount;
				}
			});
#else
			options = new Options() {Benchmark = "ZIP", Multithreaded = true, Runs = 1};
#endif

			var runner = new BenchmarkRunner(options);
			runner.RunBenchmark();

			Console.WriteLine(
				new Dictionary<string, double> {{runner.benchmark.GetDescription(), runner.lastTiming}}.ToStringTable(
					new[] {"Benchmark", "Time", "Reference (3900x)"}, d => d.Key, d => FormatTime(d.Value), d => FormatTime(runner.benchmark.GetReferenceValue())));

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