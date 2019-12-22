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

		public void Prepare()
		{
			switch (options.Benchmark.ToUpper())
			{
				case "INT":
				{
					benchmarksToRun.Add(new Integer(options));
					benchmarksToRun.Add(new Encryption(options));
					benchmarksToRun.Add(new Decryption(options));
					benchmarksToRun.Add(new CSPRNG(options));
					benchmarksToRun.Add(new HTMLParser(options));
					benchmarksToRun.Add(new JSONParser(options));

					break;
				}

				case "FLOAT":
				{
					benchmarksToRun.Add(new Float(options));
					benchmarksToRun.Add(new AVX(options));
					benchmarksToRun.Add(new SSE(options));

					break;
				}

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

					if (string.Equals(benchmark.GetName(), options.Benchmark,
						    StringComparison.CurrentCultureIgnoreCase) ||
					    string.Equals(benchmark.GetCategory(), options.Benchmark,
						    StringComparison.CurrentCultureIgnoreCase))
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
				lock (CurrentRunningBenchmark)
				{
					CurrentRunningBenchmark = benchmarksToRun[0].GetName();
					CurrentBenchmarkFinished = 0;
				}

				if (!categories.ContainsKey(benchmarksToRun[0].GetCategory()))
				{
					categories.Add(benchmarksToRun[0].GetCategory(), new List<Result>());
				}

				// Execute
				benchmarksToRun[0].Initialize();
				CurrentBenchmarkFinished = 0;
				CurrentRunningBenchmark = benchmarksToRun[0].GetName();
				var timing = ExecuteBenchmark();

				var result = new Result(
					benchmarksToRun[0].GetName(),
					timing,
					benchmarksToRun[0].GetRatingMethod().Invoke(timing, benchmarksToRun[0].GetReferenceValue()),
					benchmarksToRun[0].GetComparison(),
					benchmarksToRun[0].GetRatingMethod().Invoke(benchmarksToRun[0].GetComparison(),
						benchmarksToRun[0].GetReferenceValue())
				);

				Results.Add(result);

				categories[benchmarksToRun[0].GetCategory()].Add(result);

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
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

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

						if (benchmark.GetCategory() == keyValuePair.Key)
						{
							if (keyValuePair.Value.All(r => r.Benchmark != benchmark.GetName()))
							{
								skip = true;
								break;
							}
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