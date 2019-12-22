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
using Benchmarking.Parsing;
using Benchmarking.Results;
using Double = Benchmarking.Arithmetic.Double;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		internal static readonly List<Type> AvailableBenchmarks = new List<Type>
		{
			typeof(ZIP),
			typeof(GZip),
			typeof(BZip2),
			typeof(Deflate),
			typeof(Decompression.ZIP),
			typeof(Decompression.GZip),
			typeof(Decompression.Deflate),
			typeof(Decompression.BZip2),
//			typeof(Brotli),
			typeof(Integer),
			typeof(Float),
			typeof(Double),
			typeof(FP16),
			typeof(AVX),
			typeof(SSE),
			typeof(SSE2),
			typeof(AVX2Int),
			typeof(FMA),
			typeof(Encryption),
			typeof(Decryption),
			typeof(CSPRNG),
			typeof(HTMLParser),
			typeof(JSONParser)
		};

		private readonly List<Benchmark> benchmarksToRun = new List<Benchmark>();
		private readonly Options options;
		public readonly List<Result> Results = new List<Result>();
		private readonly long[] timings;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Runs];

			TotalOverall = options.Runs * options.Threads;
			SingleBenchmarkTotal = options.Runs * options.Threads;
		}

		public static int CurrentBenchmarkFinished { get; private set; }

		public static int FinishedOverall { get; private set; }
		public static uint TotalOverall { get; private set; }
		public static uint SingleBenchmarkTotal { get; private set; }

		public static string CurrentRunningBenchmark { get; private set; } = string.Empty;

		public static List<string> GetAvailableBenchmarks()
		{
			var available = new List<string> {"ALL"};
			var opts = new Options();

			foreach (var bench in AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(bench, opts);

				foreach (var category in benchmark.GetCategories())
				{
					if (!available.Contains(category.ToUpper()))
					{
						available.Add(category.ToUpper());
					}
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

		public void Prepare()
		{
			switch (options.Benchmark.ToUpper())
			{
				case "ALL":
				{
					foreach (var availableBenchmark in AvailableBenchmarks)
					{
						var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

						benchmarksToRun.Add(benchmark);
					}

					break;
				}
			}

			if (benchmarksToRun.Count == 0)
			{
				foreach (var availableBenchmark in AvailableBenchmarks)
				{
					var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

					if (string.Equals(benchmark.GetName(), options.Benchmark, StringComparison.CurrentCultureIgnoreCase)
					    || benchmark.GetCategories().Any(category =>
						    string.Equals(category, options.Benchmark, StringComparison.CurrentCultureIgnoreCase)))
					{
						benchmarksToRun.Add(benchmark);
					}
				}
			}

			TotalOverall *= (uint) benchmarksToRun.Count;
		}

		public void RunBenchmark()
		{
			RunGenericBenchmark();
			benchmarksToRun.Clear();
			GC.Collect();
		}

		internal static void ReportProgress()
		{
			lock (CurrentRunningBenchmark)
			{
				FinishedOverall++;
				CurrentBenchmarkFinished++;
			}
		}

		private void RunGenericBenchmark()
		{
			var categories = new Dictionary<string, List<Result>>();

			while (benchmarksToRun.Count > 0)
			{
				var currentBenchmark = benchmarksToRun[0];

				lock (CurrentRunningBenchmark)
				{
					CurrentRunningBenchmark = currentBenchmark.GetName();
					CurrentBenchmarkFinished = 0;
				}

				foreach (var category in currentBenchmark.GetCategories())
				{
					if (!categories.ContainsKey(category))
					{
						categories.Add(category, new List<Result>());
					}
				}

				// Execute
				currentBenchmark.Initialize();

				CurrentBenchmarkFinished = 0;
				CurrentRunningBenchmark = currentBenchmark.GetName();

				var timing = ExecuteBenchmark();

				var result = new Result(
					currentBenchmark.GetName(),
					timing,
					currentBenchmark.GetRatingMethod().Invoke(timing, currentBenchmark.GetReferenceValue()),
					currentBenchmark.GetComparison(),
					currentBenchmark.GetRatingMethod().Invoke(currentBenchmark.GetComparison(),
						currentBenchmark.GetReferenceValue())
				);

				Results.Add(result);

				foreach (var category in currentBenchmark.GetCategories())
				{
					categories[category].Add(result);
				}

				benchmarksToRun.RemoveAt(0);
				GC.Collect();
			}

			ProcessCategories(categories);

			// Add to save
			foreach (var runnerResult in Results)
			{
				ResultSaver.SaveResult(options.Threads, runnerResult);
			}

			// Check for newly completed categories
			ResultCategoryAggregator.ProcessCategories(options, categories);
		}

		private double ExecuteBenchmark()
		{
			var sw = new Stopwatch();
			var currentBenchmark = benchmarksToRun[0];
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

			for (var i = 0; i < options.Runs; i++)
			{
				if (options.MemoryEfficient)
				{
					sw.Start();
					currentBenchmark.Run();
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
						currentBenchmark.Run();
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

			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

			return timings.Average();
		}

		private void ProcessCategories(Dictionary<string, List<Result>> categories)
		{
			if (categories.Count > 0)
			{
				var pointss = new List<double>();
				var timingss = new List<double>();
				var refPointss = new List<double>();
				var refTimings = new List<double>();

				foreach (var keyValuePair in categories)
				{
					// Check if we got all benchmarks for that category
					var skip = false;

					foreach (var availableBenchmark in AvailableBenchmarks)
					{
						var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

						if (benchmark.GetCategories().All(category => category != keyValuePair.Key))
						{
							continue;
						}

						if (keyValuePair.Value.All(r => r.Benchmark != benchmark.GetName()))
						{
							skip = true;
							break;
						}
					}

					if (skip)
					{
						continue;
					}

					var points = 0.0d;
					var timing = 0.0d;
					var refTiming = 0.0d;
					var refPoints = 0.0d;

					foreach (var tuple in keyValuePair.Value)
					{
						points += tuple.Points;
						timing += tuple.Timing;
						refTiming += tuple.ReferenceTiming;
						refPoints += tuple.ReferencePoints;
					}

					points /= keyValuePair.Value.Count;
					refPoints /= keyValuePair.Value.Count;

					points = Math.Round(points, 0);
					refPoints = Math.Round(refPoints, 0);

					Results.Add(new Result("Category: " + keyValuePair.Key, timing, points, refTiming,
						refPoints));

					pointss.Add(points);
					timingss.Add(timing);
					refPointss.Add(refPoints);
					refTimings.Add(refTiming);
				}

				if (options.Benchmark.ToUpper() == "ALL")
				{
					var totalPoints = Math.Round(pointss.Average(), 0);
					var totalRefPoints = Math.Round(refPointss.Average(), 0);

					Results.Add(new Result("Category: " + options.Benchmark, timingss.Sum(), totalPoints,
						refTimings.Sum(),
						totalRefPoints));
				}
			}
		}
	}
}