#region using

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Benchmarking.Util;
#if NETCOREAPP3_0
using System.Runtime.Intrinsics.X86;

#endif

#endregion

namespace Benchmarking.Extension
{
	internal class AVX : Benchmark
	{
		private readonly uint numberOfIterations = 15000000u;
		private List<float[]> datas;
		private float randomFloatingNumber;

		public AVX(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Avx.IsSupported)
			{
				return;
			}

			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				threads[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					var randomFloatingSpan = new Span<float>(new[] {randomFloatingNumber});
					var dst = new Span<float>(datas[i1]);

					var iterations = numberOfIterations / options.Threads;

					for (var j = 0; j < iterations; j++)
					{
						AddScalarU(randomFloatingSpan, dst);
						MultiplyScalarU(randomFloatingSpan, dst);
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
			return "AVX benchmark of addition and multiplication on 512 floats (16384 bits)";
		}

		public override void Initialize()
		{
			randomFloatingNumber = float.Epsilon;

			datas = new List<float[]>((int) options.Threads);

			for (var i = 0; i < options.Threads; i++)
			{
				// Multiple of 256 to test AVX only
				datas.Add(new float[512]);
			}
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1100.0d;
				}
				default:
				{
					return 200.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"extension", "float"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(float) * 512 * numberOfIterations * 2 / (timeInMillis / 1000);
		}

#if NETCOREAPP3_0
		private unsafe void MultiplyScalarU(Span<float> scalar, Span<float> dst)
		{
			fixed (float* pdst = dst)
			fixed (float* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector256 = Avx.BroadcastScalarToVector256(psrc);

				while (pDstCurrent + 8 <= pDstEnd)
				{
					var dstVector = Avx.LoadVector256(pDstCurrent);
					dstVector = Avx.Multiply(dstVector, scalarVector256);
					Avx.Store(pDstCurrent, dstVector);

					pDstCurrent += 8;
				}
			}
		}

		private unsafe void AddScalarU(Span<float> scalar, Span<float> dst)
		{
			fixed (float* pdst = dst)
			fixed (float* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector256 = Avx.BroadcastScalarToVector256(psrc);

				while (pDstCurrent + 8 <= pDstEnd)
				{
					var dstVector = Avx.LoadVector256(pDstCurrent);
					dstVector = Avx.Add(dstVector, scalarVector256);
					Avx.Store(pDstCurrent, dstVector);

					pDstCurrent += 8;
				}
			}
		}
#endif
	}
}