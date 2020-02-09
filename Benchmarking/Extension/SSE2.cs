#region using

using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking.Util;
#if NETCOREAPP3_0
using System.Runtime.Intrinsics.X86;

#endif

#endregion

namespace Benchmarking.Extension
{
	internal class SSE2 : Benchmark
	{
		private readonly uint numberOfIterations = 20000000;
		private const uint randomIntegerNumber = 3;

		public SSE2(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Sse2.IsSupported)
			{
				return;
			}

			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					var randomIntegerSpan = new Span<uint>(new[] {randomIntegerNumber});
					var dst = new Span<uint>(new uint[256]);

					var iterations = numberOfIterations / options.Threads;

					for (var j = 0; j < iterations; j++)
					{
						AddScalarU(randomIntegerSpan, dst);
						MultiplyScalarU(randomIntegerSpan, dst);
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(threads);
#else
			return;
#endif
		}

		public override ulong Stress(CancellationToken cancellationToken)
		{
#if NETCOREAPP3_0
			if (!Sse2.IsSupported)
			{
				return 0uL;
			}

			var threads = new Task[options.Threads];
			var completed = 0uL;

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					var threadCompleted = 0uL;
					var randomIntegerSpan = new Span<uint>(new[] { randomIntegerNumber });
					var dst = new Span<uint>(new uint[256]);

					while (!cancellationToken.IsCancellationRequested)
					{
						AddScalarU(randomIntegerSpan, dst);
						MultiplyScalarU(randomIntegerSpan, dst);

						dst.Clear();
						threadCompleted++;
					}

					lock (threads)
					{
						completed += threadCompleted;
					}
				}, ThreadPriority.BelowNormal);
			}

			Task.WaitAll(threads);

			return completed / numberOfIterations;
#else
			return 0uL;
#endif
		}

		public override string GetDescription()
		{
			return "SSE2 benchmark of addition and multiplication on 256 integers (8192 bits)";
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 996.0d;
				}
				default:
				{
					return 262.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"extension", "int", "sse"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(uint) * 256 * numberOfIterations * 2 / (timeInMillis / 1000);
		}

#if NETCOREAPP3_0
		private unsafe void MultiplyScalarU(Span<uint> scalar, Span<uint> dst)
		{
			fixed (uint* pdst = dst)
			fixed (uint* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector128 = Sse2.LoadScalarVector128(psrc);

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Sse2.LoadVector128(pDstCurrent);
					dstVector = Sse2.Multiply(dstVector, scalarVector128).AsUInt32();
					Sse2.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}
			}
		}

		private unsafe void AddScalarU(Span<uint> scalar, Span<uint> dst)
		{
			fixed (uint* pdst = dst)
			fixed (uint* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector128 = Sse2.LoadScalarVector128(psrc);

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Sse2.LoadVector128(pDstCurrent);
					dstVector = Sse2.Add(dstVector, scalarVector128);
					Sse2.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}
			}
		}
#endif
	}
}