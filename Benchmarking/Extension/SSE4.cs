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
	internal class SSE4 : Benchmark
	{
		private readonly uint numberOfIterations = 20000000;
		private List<float[]> datas;
		private float randomFloatingNumber;

		public SSE4(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Sse41.IsSupported)
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
						DotProductU(randomFloatingSpan, dst);
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
			return "SSE4.1 benchmark of dot products on 256 floats (8192 bits)";
		}

		public override void Initialize()
		{
			randomFloatingNumber = float.Epsilon;

			datas = new List<float[]>((int) options.Threads);

			for (var i = 0; i < options.Threads; i++)
			{
				// Multiple of 128 to test SSE only
				datas.Add(new float[256]);
			}
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1475.0d;
				}
				default:
				{
					return 300.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"extension", "float", "sse"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(float) * 256 * numberOfIterations / (timeInMillis / 1000);
		}

#if NETCOREAPP3_0
		private unsafe void DotProductU(Span<float> scalar, Span<float> dst)
		{
			fixed (float* pdst = dst)
			fixed (float* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector128 = Sse.LoadScalarVector128(psrc);

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Sse.LoadVector128(pDstCurrent);
					dstVector = Sse41.DotProduct(dstVector, scalarVector128, 0xff);
					Sse.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}
			}
		}
#endif
	}
}