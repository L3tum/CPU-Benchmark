#region using

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Util;
#if NETCOREAPP3_0
using System.Runtime.Intrinsics.X86;

#endif

#endregion

namespace Benchmarking.Extension
{
	internal class SSE42CRC32C : Benchmark
	{
		private readonly List<ulong> datas;
		private readonly uint numberOfIterations = 1000000;

		public SSE42CRC32C(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);

			datas = new List<ulong>();
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Sse42.IsSupported)
			{
				return;
			}

			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					var iterations = numberOfIterations / options.Threads;

					for (var j = 0; j < iterations; j++)
					{
						CRC32C();
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(threads);
#else
			return;
#endif
		}

		public override string GetDescription()
		{
			return "SSE4.2 benchmark of CRC32C on 100 ulong (64 bits)";
		}

		public override void Initialize()
		{
			// Multiple of 128 to test SSE only
			var data = DataGenerator.GenerateString(6400);
			var bytes = Encoding.ASCII.GetBytes(data);

			for (var j = 0; j + 8 < bytes.Length; j += 8)
			{
				var character = (ulong) ((long) bytes[j] << 56) + ((ulong) bytes[j + 1] << 48) +
				                ((ulong) bytes[j + 2] << 40) + ((ulong) bytes[j + 3] << 32) +
				                ((ulong) bytes[j + 4] << 24) + ((ulong) bytes[j + 5] << 16) +
				                ((ulong) bytes[j + 6] << 8) + bytes[j + 7];

				datas.Add(character);
			}
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1571.0d;
				}
				default:
				{
					return 300.0d;
				}
			}
		}

		public override string GetName()
		{
			return "sse4-crc32c";
		}

		public override string[] GetCategories()
		{
			return new[] {"extension", "int", "sse", "cryptography"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(ulong) * 100 * numberOfIterations / (timeInMillis / 1000);
		}

#if NETCOREAPP3_0
		private void CRC32C()
		{
			var crc = 0uL;

			foreach (var character in datas)
			{
				crc = Sse42.X64.Crc32(crc, character);
			}
		}
#endif
	}
}