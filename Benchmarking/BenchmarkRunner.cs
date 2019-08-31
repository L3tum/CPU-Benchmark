#region using

using System;
using System.Diagnostics;
using System.Linq;
using Benchmarking.Compression;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		public static string[] AvailableBenchmarks = {"ZIP", "GZIP", "BZIP2", "DEFLATE", "ZIPCOMPRESSORS"};
		private static int finished;
		private static int total;
		private static object _lock = new object();
		private readonly Options options;
		private readonly long[] timings;
		public double lastTiming;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Runs];

			total = options.Runs * options.Threads;
		}

		public static double CurrentProgress { get; private set; }

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

		internal static void ReportProgress()
		{
			lock (_lock)
			{
				finished++;
				CurrentProgress = (double) finished / total;
			}
		}

		private void RunGenericBenchmark()
		{
			benchmark.Initialize();

			lastTiming = ExecuteBenchmark();
		}

		private double ExecuteBenchmark()
		{
			var sw = new Stopwatch();

			for (var i = 0; i < options.Runs; i++)
			{
				sw.Start();
				benchmark.Run();
				sw.Stop();

				benchmark.PostRun();
				GC.Collect();

				timings[i] = sw.ElapsedMilliseconds;
				sw.Reset();
			}

			return timings.Average();
		}
	}
}