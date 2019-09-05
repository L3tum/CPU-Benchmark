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

			// if time is higher, subtract reference time and get value on a linear function
			if (timeInMillis > referenceTimeInMillis)
			{
				return Math.Round(-0.5 * (timeInMillis - referenceTimeInMillis) + baseline, 0);
			}

			// Calculate the graph going through (0, 50000) and (referenceTimeInMillis, 0) (y = mx+b)
			if (timeInMillis < referenceTimeInMillis)
			{
				var m = (baseline - 0) / (0 - referenceTimeInMillis);
				var b = 0 - m * referenceTimeInMillis;

				return Math.Round(m * timeInMillis + b, 0) + baseline;
			}

			return baseline;
		}
	}
}