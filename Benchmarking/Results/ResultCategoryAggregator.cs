#region using

using System;
using System.Collections.Generic;
using System.Linq;
using CPU_Benchmark_Common;

#endregion

namespace Benchmarking.Results
{
	internal static class ResultCategoryAggregator
	{
		internal static void ProcessCategories(Options options, Dictionary<string, List<Result>> categories)
		{
			var registeredCategories = new Dictionary<string, List<string>>();
			var finishedBenchmarks = new Dictionary<string, List<Result>>();

			// Collect all categories and the respective benchmarks
			foreach (var availableBenchmark in BenchmarkRunner.AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

				foreach (var category in benchmark.GetCategories())
				{
					if (!registeredCategories.ContainsKey(category))
					{
						registeredCategories.Add(category, new List<string>());
					}

					registeredCategories[category].Add(benchmark.GetName());
				}
			}

			// Add all already run benchmarks except categories
			var alreadyRunBenchmarks = ResultSaver.GetResults();

			if (alreadyRunBenchmarks.ContainsKey(options.Threads))
			{
				foreach (var result in alreadyRunBenchmarks[options.Threads])
				{
					if (result.Benchmark.ToLowerInvariant().StartsWith("category:"))
					{
						continue;
					}

					var category = registeredCategories.Keys.FirstOrDefault(key =>
						registeredCategories[key].Any(bench =>
							string.Equals(bench, result.Benchmark, StringComparison.InvariantCultureIgnoreCase)));

					if (category != null)
					{
						if (!finishedBenchmarks.ContainsKey(category))
						{
							finishedBenchmarks.Add(category, new List<Result>());
						}

						finishedBenchmarks[category].Add(result);
					}
				}
			}

			// Add all new benchmarks
			foreach (var keyValuePair in categories)
			{
				if (!finishedBenchmarks.ContainsKey(keyValuePair.Key))
				{
					finishedBenchmarks.Add(keyValuePair.Key, new List<Result>());
				}

				foreach (var result in keyValuePair.Value)
				{
					var oldBenchmark = finishedBenchmarks[keyValuePair.Key]
						.FirstOrDefault(oldResult => oldResult.Benchmark == result.Benchmark);

					if (oldBenchmark != null)
					{
						finishedBenchmarks[keyValuePair.Key].Remove(oldBenchmark);
					}

					finishedBenchmarks[keyValuePair.Key].Add(result);
				}
			}

			foreach (var finishedBenchmark in finishedBenchmarks)
			{
				// Check if we got all required benchmarks for this category
				if (!registeredCategories[finishedBenchmark.Key].All(benchmark => finishedBenchmark.Value.Any(
					finished =>
						string.Equals(finished.Benchmark, benchmark, StringComparison.InvariantCultureIgnoreCase))))
				{
					// Don't have all, just skips this
					continue;
				}

				// Calculate average
				var points = Math.Round(finishedBenchmark.Value.Average(benchmark => benchmark.Points), 0);
				var timing = Math.Round(finishedBenchmark.Value.Average(benchmark => benchmark.Timing), 0);
				var referencePoints =
					Math.Round(finishedBenchmark.Value.Average(benchmark => benchmark.ReferencePoints), 0);
				var referenceTiming =
					Math.Round(finishedBenchmark.Value.Average(benchmark => benchmark.ReferenceTiming), 0);
				var throughput = Math.Round(finishedBenchmark.Value.Average(benchmark => benchmark.DataThroughput), 0);

				ResultSaver.SaveResult(options.Threads,
					new Result($"Category: {finishedBenchmark.Key}", timing, points, referenceTiming, referencePoints, throughput));
			}

			// Special case for "all"
			var allBenchmarks = new List<Result>();

			finishedBenchmarks.ToList().ForEach(kvp => allBenchmarks.AddRange(kvp.Value));

			// Check if we got all benchmarks
			if (BenchmarkRunner.AvailableBenchmarks.All(availableBenchmark => allBenchmarks.Any(bench =>
				string.Equals(bench.Benchmark,
					((Benchmark) Activator.CreateInstance(availableBenchmark, options)).GetName(),
					StringComparison.InvariantCultureIgnoreCase))))
			{
				// Calculate average
				var points = Math.Round(allBenchmarks.Average(benchmark => benchmark.Points), 0);
				var timing = Math.Round(allBenchmarks.Average(benchmark => benchmark.Timing), 0);
				var referencePoints =
					Math.Round(allBenchmarks.Average(benchmark => benchmark.ReferencePoints), 0);
				var referenceTiming =
					Math.Round(allBenchmarks.Average(benchmark => benchmark.ReferenceTiming), 0);
				var throughput = Math.Round(allBenchmarks.Average(benchmark => benchmark.DataThroughput), 0);

				ResultSaver.SaveResult(options.Threads,
					new Result("Category: all", timing, points, referenceTiming, referencePoints, throughput));
			}
		}
	}
}