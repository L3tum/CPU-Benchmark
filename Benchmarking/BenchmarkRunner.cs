#region using

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		private readonly Options options;
		private readonly long[] timings;
		public double lastTiming;
		public Benchmark benchmark;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Repeated];
		}

		public void RunZipBenchmark()
		{
			if (benchmark == null || typeof(Benchmark) != typeof(ZIP.ZIP))
			{
				benchmark = new ZIP.ZIP(options);
			}

			RunGenericBenchmark();
		}

		private void RunGenericBenchmark()
		{
			InitializeBenchmark(benchmark.Initialize);

			lastTiming = RunBenchmark(benchmark.Run);
		}

		private void InitializeBenchmark(Action action)
		{
			Console.WriteLine("Initializing Benchmark...");
			action.Invoke();
		}

		private double RunBenchmark(Action action)
		{
			Console.WriteLine($"Running Benchmark {options.Repeated} times on {options.Threads} threads...");
			var sw = new Stopwatch();
			var ct = new CancellationTokenSource();
			var t = Task.Run(() =>
			{
				Console.Write("..");

				while (!ct.IsCancellationRequested)
				{
					Console.Write(".");
					Thread.Sleep(500);
					Console.CursorLeft -= 1;
					Console.Write(" ");
					Thread.Sleep(500);
					Console.CursorLeft -= 1;
				}

				Console.WriteLine();
			}, ct.Token);

			for (var i = 0; i < options.Repeated; i++)
			{
				sw.Start();
				action.Invoke();
				sw.Stop();

				timings[i] = sw.ElapsedMilliseconds;
				sw.Reset();
			}

			ct.Cancel();
			t.ConfigureAwait(false).GetAwaiter().GetResult();

			return timings.Average();
		}
	}
}