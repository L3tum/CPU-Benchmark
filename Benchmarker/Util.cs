#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Benchmarking.Results;
using Benchmarking.Util;

#endregion

namespace Benchmarker
{
    internal static class Util
    {
        internal static string FormatResults(Dictionary<int, List<Result>> results)
        {
            var s = string.Empty;

            foreach (var keyValuePair in results)
            {
                s += $"Benchmarked on {keyValuePair.Key} Threads\n";

                s += keyValuePair.Value.ToStringTable(
                    new[] {"Benchmark", "Points", "Reference", "DataThroughput", "AverageFrequency", "HighestFrequency"},
                    r => r.Benchmark,
                    r => r.Iterations,
                    r => r.ReferenceIterations,
                    r => $"{Helper.FormatBytes((ulong) r.DataThroughput)}/s",
                    r => $"{r.Frequency?.AverageFrequency} MHz",
                    r => $"{r.Frequency?.HighestFrequency} MHz"
                );
            }

            return s;
        }
    }
}