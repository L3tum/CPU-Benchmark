#region using

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking.Compression;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		public static string[] AvailableBenchmarks = {"ZIP", "GZIP", "BZIP2", "DEFLATE", "ZIPCOMPRESSORS"};
		private readonly Options options;
		private readonly long[] timings;
		public double lastTiming;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Runs];
		}

		public Benchmark benchmark { get; private set; }

		public void RunBenchmark()
		{
			switch (options.Benchmark.ToUpper())
			{
				case "ZIP":
				{
					benchmark = new ZIP(options);

					break;
				}

				case "GZIP":
				{
					benchmark = new GZip(options);

					break;
				}

				case "BZIP2":
				{
					benchmark = new BZip2(options);

					break;
				}

				case "DEFLATE":
				{
					benchmark = new Deflate(options);

					break;
				}

				case "ZIPCOMPRESSORS":
				{
					benchmark = new ZIPCompressors(options);

					break;
				}

				default:
				{
					throw new ArgumentException($"Benchmark {options.Benchmark} to recognized!");
				}
			}

			RunGenericBenchmark();
		}

		private void RunGenericBenchmark()
		{
			InitializeBenchmark();

			lastTiming = ExecuteBenchmark();
		}

		private void InitializeBenchmark()
		{
			Console.WriteLine("Initializing Benchmark...");
			benchmark.Initialize();
		}

		private double ExecuteBenchmark()
		{
			Console.WriteLine($"Running Benchmark {options.Runs} times on {options.Threads} threads...");
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

			for (var i = 0; i < options.Runs; i++)
			{
				sw.Start();
				benchmark.Run();
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