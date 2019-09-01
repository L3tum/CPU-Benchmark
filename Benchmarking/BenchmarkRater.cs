using System;

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		// Maybe replace with linear rating?
		public static double RateBenchmark(double timeInMillis)
		{
			// Maximum 1000000 points (100k)
			const double maxTime = 100000;

			// "Maximum" of 10000000 (1M) milliseconds or 1000 seconds

			// Nice exponential graph from x = 0 to x ~ 1000000 and
			return Math.Round(maxTime * Math.Pow(0.5, 0.00001 * timeInMillis), 0);
		}
	}
}