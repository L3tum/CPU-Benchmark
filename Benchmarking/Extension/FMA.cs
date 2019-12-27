#region using

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if NETCOREAPP3_0
using System.Runtime.Intrinsics.X86;

#endif

#endregion

namespace Benchmarking.Extension
{
	internal class FMA : Benchmark
	{
		private readonly uint numberOfIterations = 50000000;
		private List<float[]> datas;
		private float randomFloatingNumber;

		public FMA(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Fma.IsSupported)
			{
				return;
			}

			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				threads[i] = Task.Run(() =>
				{
					var randomFloatingSpan = new Span<float>(new[] {randomFloatingNumber});
					var dst = new Span<float>(datas[i1]);

					var iterations = numberOfIterations / options.Threads;

					for (var j = 0; j < iterations; j++)
					{
						AddMulScalarU(randomFloatingSpan, dst);
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
			return "SSE benchmark of fused addition and multiplication on 256 floats (8192 bits)";
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
					return 1213.0d;
				}
				default:
				{
					return 351.0d;
				}
			}
		}

#if NETCOREAPP3_0
		private unsafe void AddMulScalarU(Span<float> scalar, Span<float> dst)
		{
			fixed (float* pdst = dst)
			fixed (float* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector128 = Sse.LoadScalarVector128(psrc);

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Fma.LoadVector128(pDstCurrent);
					dstVector = Fma.MultiplyAdd(dstVector, scalarVector128, scalarVector128);
					Fma.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}
			}
		}
#endif
		public override string[] GetCategories()
		{
			return new[] {"extension", "float"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(float) * 256 * numberOfIterations * 2 / (timeInMillis / 1000);
		}
	}
}