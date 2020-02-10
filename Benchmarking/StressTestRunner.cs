#region using

using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

#endregion

namespace Benchmarking
{
	public class StressTestRunner
	{
		private readonly Options options;
		private Benchmark benchmarkToRun;

		public StressTestRunner(Options options)
		{
			this.options = options;
		}

		public void Prepare()
		{
			foreach (var availableBenchmark in BenchmarkRunner.AvailableBenchmarks)
			{
				var benchmark = (Benchmark) Activator.CreateInstance(availableBenchmark, options);

				if (string.Equals(benchmark.GetName(), options.Benchmark, StringComparison.CurrentCultureIgnoreCase))
				{
					benchmarkToRun = benchmark;

					break;
				}
			}
		}

		public ulong RunStressTest(CancellationToken cancellationToken)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			var completed = 0uL;

			if (options.MemoryEfficient)
			{
				completed = benchmarkToRun.Stress(cancellationToken);
			}
			else
			{
				var oldMode = GCSettings.LatencyMode;

				// Make sure we can always go to the catch block, 
				// so we can set the latency mode back to `oldMode`
				RuntimeHelpers.PrepareConstrainedRegions();

				try
				{
					GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

					// Generation 2 garbage collection is now
					// deferred, except in extremely low-memory situations
					completed = benchmarkToRun.Stress(cancellationToken);
				}
				finally
				{
					// ALWAYS set the latency mode back
					GCSettings.LatencyMode = oldMode;
				}
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			return completed;
		}
	}
}