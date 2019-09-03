#region using

using System;

#endregion

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		// Maybe replace with linear rating?
		public static double RateBenchmark(double timeInMillis, double referenceTimeInMillis)
		{
			const int baseline = 50000;

			if (timeInMillis > referenceTimeInMillis)
			{
				return Math.Round(-0.5 * timeInMillis + baseline, 0);
			}

			if (timeInMillis < referenceTimeInMillis)
			{
				return Math.Round(-0.5 * timeInMillis + baseline, 0) + baseline;
			}

			return baseline;
		}
	}
}