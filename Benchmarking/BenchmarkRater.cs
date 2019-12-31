#region using

using System;

#endregion

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		public delegate double RateMethod(double timeInMillis);
		private const int baseline = 10000;

		public static double RateBenchmark(double timeInMillis)
		{
			var step = 1000;
			var pointsPerMilli = 2.0d;
			double points = baseline;

			while (timeInMillis > 0.0)
			{
				if (timeInMillis > step)
				{
					points -= (step * pointsPerMilli);
				}
				else
				{
					points -= (timeInMillis * pointsPerMilli);
				}

				timeInMillis -= step;
				pointsPerMilli /= 2.0d;
			}

			return Math.Round(points, 0);
		}

		public static uint ScaleVolume(uint threads)
		{
			var scale = threads / 2;

			return scale > 0 ? scale : 1;
		}
	}
}