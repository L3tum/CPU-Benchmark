#region using

using System;
using System.Linq;

#endregion

namespace Benchmarking.Util
{
	internal class DataGenerator
	{
		internal static string GenerateString(int length)
		{
			var random = new Random();
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
		}
	}
}