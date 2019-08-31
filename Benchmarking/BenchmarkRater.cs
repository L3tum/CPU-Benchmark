using System;

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		// Maybe replace with linear rating?
		public static double RateBenchmark(double timeInMillis)
		{
			// 1 minute as upper bound
			const double maxTime = 60000;

			// Nice exponential graph from x = 0 to x ~ 100000 and
			return Math.Round(maxTime * Math.Pow(0.5, 0.0001 * timeInMillis), 0);
		}
	}
}