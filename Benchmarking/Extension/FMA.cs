﻿#region using

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
		private List<float[]> datas;
		private float randomFloatingNumber;

		public FMA(Options options) : base(options)
		{
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

					var iterations = 100000000 / options.Threads;

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
			return "SSE benchmark of fused addition and multiplication on 256 floats (1024 bits) 100.000.000 times.";
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
					return 3681.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
#if NETCOREAPP3_0
			if (!Fma.IsSupported)
			{
				return 0.0d;
			}

			return 482.0d;
#else
			return 0.0d;
#endif
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
					var dstVector = Sse.LoadVector128(pDstCurrent);
					dstVector = Fma.MultiplyAdd(dstVector, scalarVector128, scalarVector128);
					Sse.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}
			}
		}
#endif
		public override string[] GetCategories()
		{
			return new[] { "extension", "float" };
		}
	}
}