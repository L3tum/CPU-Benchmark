#region using

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking.Util;
using HardwareInformation.Information.Cpu;

#endregion

namespace Benchmarking.Latency
{
	internal class L2CacheLatency : Benchmark
	{
		private readonly int runs = 100;
		private int len;
		private volatile IntPtr pointer;
		private int stepSize = 1;

		public L2CacheLatency(Options options) : base(options)
		{
		}

		public override string[] GetCategories()
		{
			return new[] {"experimental", "latency"};
		}

		public override string GetName()
		{
			return base.GetName() + "-experimental";
		}

		public override void Run()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = ThreadAffinity.RunAffinity(1uL << i, TestLatency);
			}

			Task.WaitAll(threads);
		}

		public override void Shutdown()
		{
			Marshal.FreeHGlobal(pointer);
		}

		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		private void TestLatency()
		{
			var index = 0;
			var overhead = 0L;
			var stopwatch = new Stopwatch();
			Span<int> mem = null;

			Thread.MemoryBarrier();

			// Load into memory

			stopwatch.Restart();

			Thread.MemoryBarrier();

			for (var i = 0; i < runs; i++)
			{
				Thread.MemoryBarrier();

				Thread.MemoryBarrier();
			}

			Thread.MemoryBarrier();

			stopwatch.Stop();

			Thread.MemoryBarrier();

			// Check overhead
			stopwatch.Restart();

			Thread.MemoryBarrier();

			for (var i = 0; i < runs; i++)
			{
				Thread.MemoryBarrier();

				Thread.MemoryBarrier();
			}

			Thread.MemoryBarrier();

			stopwatch.Stop();

			Thread.MemoryBarrier();

			overhead = stopwatch.ElapsedTicks;

			unsafe
			{
				mem = new Span<int>(pointer.ToPointer(), len);

				Thread.MemoryBarrier();

				// Iterate over each one to get into memory
				fixed (int* p = mem)
				{
					for (var i = 0; i < len; i++)
					{
						p[i] = p[i];
					}
				}

				Thread.MemoryBarrier();

				stopwatch.Restart();

				Thread.MemoryBarrier();

				fixed (int* p = mem)
				{
					for (var i = 0; i < runs; i++)
					{
						Thread.MemoryBarrier();

						index = p[index];

						Thread.MemoryBarrier();
					}
				}

				Thread.MemoryBarrier();

				stopwatch.Stop();

				Thread.MemoryBarrier();
			}

			Debug.WriteLine(
				$"Time: {(stopwatch.ElapsedTicks - overhead) / ((double) Stopwatch.Frequency / 1e9) / (double) runs}ns");
			Debug.WriteLine($"Overhead: {overhead}");
			Debug.WriteLine($"Ticks: {stopwatch.ElapsedTicks}");
			Debug.WriteLine($"ns/tick: {Stopwatch.Frequency / 1e9}");

			File.WriteAllText("./nanoseconds.txt",
				$"Time: {(stopwatch.ElapsedTicks - overhead) / ((double) Stopwatch.Frequency / 1e9) / (double) runs}ns");
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return runs * sizeof(int) / (timeInMillis / 1000);
		}

		public override void Initialize()
		{
			var lowerCache = BenchmarkRunner.MachineInformation.Cpu.Caches.FirstOrDefault(cache =>
				cache.Level == Cache.CacheLevel.LEVEL1 && cache.Type == Cache.CacheType.DATA);
			var foundCache = BenchmarkRunner.MachineInformation.Cpu.Caches.FirstOrDefault(cache =>
				cache.Level == Cache.CacheLevel.LEVEL2);

			if (foundCache != null && lowerCache != null)
			{
				len = (int) foundCache.Capacity / sizeof(int) / (int) foundCache.CoresPerCache;

				// StepSize of one cache level down to avoid it
				stepSize = (int) lowerCache.Capacity / sizeof(int) / (int) lowerCache.CoresPerCache;

				//len = (int) lowerCache.Capacity / sizeof(int) / (int) lowerCache.CoresPerCache;
				//stepSize = 2;

				Debug.WriteLine(foundCache.CapacityHRF);
				Debug.WriteLine(Helper.FormatBytes((ulong) len * sizeof(int)));
				Debug.WriteLine(lowerCache.CapacityHRF);
				Debug.WriteLine(Helper.FormatBytes((ulong) stepSize * sizeof(int)));

				pointer = Marshal.AllocHGlobal(len * sizeof(int));
				Span<int> mem = null;

				unsafe
				{
					mem = new Span<int>(pointer.ToPointer(), len);
				}

				for (var j = 0; j < len; j++)
				{
					if (j + stepSize >= len)
					{
						mem[j] = j - len + stepSize;
					}
					else
					{
						mem[j] = j + stepSize;
					}
				}
			}
		}
	}
}