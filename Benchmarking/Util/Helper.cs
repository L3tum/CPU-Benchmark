#region using

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Benchmarking.Util
{
	public static class Helper
	{
		internal static DateTime Trim(this DateTime date, long ticks)
		{
			return new DateTime(date.Ticks - date.Ticks % ticks, date.Kind);
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
	}
}