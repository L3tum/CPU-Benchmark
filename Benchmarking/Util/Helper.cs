#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#endregion

namespace Benchmarking.Util
{
    public static class Helper
    {
        internal static DateTime Trim(this DateTime date, long ticks)
        {
            return new(date.Ticks - date.Ticks % ticks, date.Kind);
        }

        internal static byte[] ReadAllBytes(this BinaryReader reader)
        {
            using var ms = new MemoryStream();
            reader.BaseStream.CopyTo(ms);
            return ms.ToArray();
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static string FormatBytes(ulong bytes)
        {
            string[] Suffix = {"B", "KiB", "MiB", "GiB", "TiB", "PiB"};
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        // Taken from: https://stackoverflow.com/a/18375526/5155536
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            // null does not have base type
            if (type == null)
            {
                return false;
            }

            // only interface or object can have null base type
            if (baseType == null)
            {
                return type.IsInterface || type == typeof(object);
            }

            // check implemented interfaces
            if (baseType.IsInterface)
            {
                return type.GetInterfaces().ToList().Contains(baseType);
            }

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        public static double GetMedian(IEnumerable<double> sourceNumbers)
        {
            var numbers = sourceNumbers as double[] ?? sourceNumbers.ToArray();
            if (!numbers.Any())
            {
                throw new Exception("No median of empty array");
            }

            // sort the numbers
            var sortedNumbers = (double[]) numbers.Clone();
            Array.Sort(sortedNumbers);

            //get the median
            var size = sortedNumbers.Length;
            var mid = size / 2;
            var median = size % 2 != 0 ? sortedNumbers[mid] : (sortedNumbers[mid] + sortedNumbers[mid - 1]) / 2;
            return median;
        }

        public static string FormatTime(double time)
        {
            var ts = TimeSpan.FromMilliseconds(time);

            var parts = $"{ts.Days:D2}d:{ts.Hours:D2}h:{ts.Minutes:D2}m:{ts.Seconds:D2}s:{ts.Milliseconds:D3}ms"
                .Split(':')
                .SkipWhile(s => Regex.Match(s, @"^00\w").Success) // skip zero-valued components
                .ToArray();
            return string.Join(" ", parts); // combine the result
        }

        public static double GetGeometricMean(IEnumerable<double> values)
        {
            var doubles = values as double[] ?? values.ToArray();
            var total = doubles.Aggregate(1.0d, (current, value) => current * value);

            return Math.Pow(total, 1.0 / doubles.Length);
        }

        public static ulong GetGeometricMean(IEnumerable<ulong> values)
        {
            return (ulong) GetGeometricMean(values.Cast<double>());
        }
    }
}