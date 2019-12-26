﻿#region using

using System;

#endregion

namespace Benchmarking
{
	public static class BenchmarkRater
	{
		public delegate double RateMethod(double timeInMillis);

		public static double RateBenchmark(double timeInMillis)
		{
			const int baseline = 10000;

			return Math.Round(baseline - baseline * timeInMillis / (baseline + timeInMillis), 0);
		}
	}
}