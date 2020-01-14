#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Benchmarking.Util;
using CPU_Benchmark_Common;

#endregion

namespace Benchmarker
{
	internal static class Util
	{
		internal static string FormatResults(Dictionary<uint, List<Result>> results)
		{
			var s = string.Empty;

			foreach (var keyValuePair in results)
			{
				s += $"Benchmarked on {keyValuePair.Key} Threads\n";

				s += keyValuePair.Value.ToStringTable(
					new[] {"Benchmark", "Time", "Reference", "Points", "Reference", "DataThroughput"},
					r => r.Benchmark,
					r => FormatTime(r.Timing),
					r => FormatTime(r.ReferenceTiming),
					r => r.Points,
					r => r.ReferencePoints,
					r => $"{Helper.FormatBytes((ulong) r.DataThroughput)}/s");
			}

			return s;
		}

		internal static string FormatTime(double time)
		{
			var ts = TimeSpan.FromMilliseconds(time);

			var parts = $"{ts.Days:D2}d:{ts.Hours:D2}h:{ts.Minutes:D2}m:{ts.Seconds:D2}s:{ts.Milliseconds:D3}ms"
				.Split(':')
				.SkipWhile(s => Regex.Match(s, @"^00\w").Success) // skip zero-valued components
				.ToArray();
			return string.Join(" ", parts); // combine the result
		}
	}
}