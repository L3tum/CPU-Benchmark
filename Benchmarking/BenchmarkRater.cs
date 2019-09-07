#region using

using System;

#endregion

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		public delegate double RateMethod(double timeInMillis, double referenceTimeInMillis);

		// Maybe replace with linear rating?
		public static double RateBenchmark(double timeInMillis, double referenceTimeInMillis)
		{
			const int baseline = 50000;

			// if time is higher, subtract reference time and get value on an exponential function
			if (timeInMillis > referenceTimeInMillis)
			{
				return Math.Round(baseline * Math.Pow(0.5, 0.00001 * timeInMillis), 0);
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