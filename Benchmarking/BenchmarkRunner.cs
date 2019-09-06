#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using Benchmarking.Arithmetic;
using Benchmarking.Compression;
using Benchmarking.Cryptography;
using Benchmarking.Extension;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		private static int finished;
		private static int total;
		private static readonly object _lock = new object();

		private static readonly List<Type> AvailableBenchmarks = new List<Type>
		{
			typeof(ZIP),
			typeof(GZip),
			typeof(BZip2),
			typeof(Deflate),
			typeof(Brotli),
			typeof(Integer),
			typeof(Float),
			typeof(AVX),
			typeof(SSE),
			typeof(Encryption),
			typeof(Decryption),
			typeof(CSPRNG)
		};

		private readonly List<Benchmark> benchmarksToRun = new List<Benchmark>();
		private readonly Options options;
		public readonly List<Result> Results = new List<Result>();
		private readonly long[] timings;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Runs];

			total = options.Runs * options.Threads;
		}

		public static double CurrentProgress { get; private set; }

		public static List<string> GetAvailableBenchmarks()
		{
			var available = new List<string>();
			var opts = new Options();

			foreach (var bench in AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(bench, opts);

				if (!available.Contains(benchmark.GetCategory().ToUpper()))
				{
					available.Add(benchmark.GetCategory().ToUpper());
				}
			}

			foreach (var bench in AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(bench, opts);

				if (!available.Contains(benchmark.GetName().ToUpper()) &&
				    !available.Contains(benchmark.GetName().ToLower()))
				{
					available.Add(benchmark.GetName().ToLower());
				}
			}

			available = available.Distinct().ToList();

			available.Remove("none");

			return available;
		}

		public void RunBenchmark()
		{
			foreach (var availableBenchmark in AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

				if (string.Equals(benchmark.GetName(), options.Benchmark, StringComparison.CurrentCultureIgnoreCase) ||
				    string.Equals(benchmark.GetCategory(), options.Benchmark,
					    StringComparison.CurrentCultureIgnoreCase))
				{
					benchmarksToRun.Add(benchmark);
				}
			}

			switch (options.Benchmark.ToUpper())
			{
				case "INT":
				{
					benchmarksToRun.Add(new Encryption(options));
					benchmarksToRun.Add(new Decryption(options));
					benchmarksToRun.Add(new CSPRNG(options));

					break;
				}

				case "FLOAT":
				{
					benchmarksToRun.Add(new AVX(options));
					benchmarksToRun.Add(new SSE(options));

					break;
				}
			}

			total *= benchmarksToRun.Count;

			RunGenericBenchmark();
			benchmarksToRun.Clear();
			GC.Collect();
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
			var categories = new Dictionary<string, List<Tuple<double, double>>>();

			while (benchmarksToRun.Count > 0)
			{
				benchmarksToRun[0].Initialize();
				var timing = ExecuteBenchmark();

				Results.Add(
					new Result(
						benchmarksToRun[0].GetName(),
						timing,
						benchmarksToRun[0].GetRatingMethod().Invoke(timing, benchmarksToRun[0].GetReferenceValue()),
						benchmarksToRun[0].GetReferenceValue(),
						benchmarksToRun[0].GetRatingMethod().Invoke(benchmarksToRun[0].GetReferenceValue(),
							benchmarksToRun[0].GetReferenceValue())
					));

				if (!categories.ContainsKey(benchmarksToRun[0].GetCategory()))
				{
					categories.Add(benchmarksToRun[0].GetCategory(), new List<Tuple<double, double>>());
				}

				categories[benchmarksToRun[0].GetCategory()]
					.Add(Tuple.Create(Results.Last().Timing, Results.Last().ReferenceTiming));

				benchmarksToRun.RemoveAt(0);
				GC.Collect();
			}

			if (Results.Count > 1)
			{
				if (options.Benchmark.ToUpper() == "ALL")
				{
					foreach (var keyValuePair in categories)
					{
						if (string.Equals(keyValuePair.Key, options.Benchmark,
							StringComparison.CurrentCultureIgnoreCase))
						{
							var timing = 0.0d;
							var refTiming = 0.0d;

							foreach (var tuple in keyValuePair.Value)
							{
								timing += tuple.Item1;
								refTiming += tuple.Item2;
							}

							timing /= keyValuePair.Value.Count;
							refTiming /= keyValuePair.Value.Count;

							var points = BenchmarkRater.RateBenchmark(timing, refTiming);
							var refPoints = BenchmarkRater.RateBenchmark(refTiming, refTiming);

							Results.Add(new Result("Category: " + keyValuePair.Key, timing, points, refTiming,
								refPoints));
						}
					}
				}
				else
				{
					var timings = new List<double>();
					var refTimings = new List<double>();

					foreach (var keyValuePair in categories)
					{
						var timing = 0.0d;
						var refTiming = 0.0d;

						foreach (var tuple in keyValuePair.Value)
						{
							timing += tuple.Item1;
							refTiming += tuple.Item2;
						}

						timing /= keyValuePair.Value.Count;
						refTiming /= keyValuePair.Value.Count;

						timings.Add(timing);
						refTimings.Add(refTiming);
					}

					var time = timings.Sum() / timings.Count;
					var refTime = refTimings.Sum() / refTimings.Count;

					var points = BenchmarkRater.RateBenchmark(time, refTime);
					var refPoints = BenchmarkRater.RateBenchmark(refTime, refTime);

					Results.Add(new Result("Category: " + options.Benchmark, time, points, refTime,
						refPoints));
				}
			}
		}

		private double ExecuteBenchmark()
		{
			var sw = new Stopwatch();

			for (var i = 0; i < options.Runs; i++)
			{
				if (options.MemoryEfficient)
				{
					sw.Start();
					benchmarksToRun[0].Run();
					sw.Stop();
				}
				else
				{
					var oldMode = GCSettings.LatencyMode;

					// Make sure we can always go to the catch block, 
					// so we can set the latency mode back to `oldMode`
					RuntimeHelpers.PrepareConstrainedRegions();

					try
					{
						GCSettings.LatencyMode = GCLatencyMode.LowLatency;

						// Generation 2 garbage collection is now
						// deferred, except in extremely low-memory situations
						sw.Start();
						benchmarksToRun[0].Run();
						sw.Stop();
					}
					finally
					{
						// ALWAYS set the latency mode back
						GCSettings.LatencyMode = oldMode;
					}
				}

				GC.Collect();

				timings[i] = sw.ElapsedMilliseconds;
				sw.Reset();
			}

			return timings.Average();
		}
	}
}