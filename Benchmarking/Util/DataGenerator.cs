#region using

using System;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion

namespace Benchmarking.Util
{
	internal class DataGenerator
	{
		private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private static readonly Random random = new Random();
		private static string cache = "";

		[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
		internal static string GenerateString(int length)
		{
			lock (cache)
			{
				if (cache.Length < length)
				{
					cache += new string(Enumerable.Range(1, length - cache.Length)
						.Select(_ => chars[random.Next(chars.Length)]).ToArray());
				}

				return new string(cache.Take(length).ToArray());
			}
		}
	}
}