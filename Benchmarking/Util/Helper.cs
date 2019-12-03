#region using

using System;

#endregion

namespace Benchmarking.Util
{
	internal static class Helper
	{
		internal static DateTime Trim(this DateTime date, long ticks)
		{
			return new DateTime(date.Ticks - date.Ticks % ticks, date.Kind);
		}
	}
}