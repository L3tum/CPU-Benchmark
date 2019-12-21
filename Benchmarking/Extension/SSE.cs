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
	internal class SSE : Benchmark
	{
		private List<float[]> datas;
		private float randomFloatingNumber;

		public SSE(Options options) : base(options)
		{
#if NETCOREAPP3_0
			if (!Sse.IsSupported)
			{
				throw new NotSupportedException("Your hardware does not support SSE intrinsics!");
			}
#else
			throw new NotSupportedException("You need at least .NET Core 3 to use this benchmark!");
#endif
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				threads[i] = Task.Run(() =>
				{
					var randomFloatingSpan = new Span<float>(new[] { randomFloatingNumber });
					var dst = new Span<float>(datas[i1]);

					var iterations = 100000000 / options.Threads;

					for (var j = 0; j < iterations; j++)
					{
						AddScalarU(randomFloatingSpan, dst);
						MultiplyScalarU(randomFloatingSpan, dst);
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(threads);
#endif
		}

		public override string GetDescription()
		{
			return "SSE benchmark of addition and multiplication on 256 floats (1024 bits) 100.000.000 times.";
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
					return 17154.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 3440.0d;
		}

#if NETCOREAPP3_0
		private unsafe void MultiplyScalarU(Span<float> scalar, Span<float> dst)
		{
			fixed (float* pdst = dst)
			fixed (float* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector128 = Sse.LoadScalarVector128(psrc);

				if (pDstCurrent + 4 <= pDstEnd)
				{
					var dstVector = Sse.LoadVector128(pDstCurrent);
					dstVector = Sse.Multiply(dstVector, scalarVector128);
					Sse.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Sse.LoadScalarVector128(pDstCurrent);
					dstVector = Sse.MultiplyScalar(dstVector, scalarVector128);
					Sse.StoreScalar(pDstCurrent, dstVector);

					pDstCurrent++;
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

				var scalarVector128 = Sse.LoadScalarVector128(psrc);

				if (pDstCurrent + 4 <= pDstEnd)
				{
					var dstVector = Sse.LoadVector128(pDstCurrent);
					dstVector = Sse.Add(dstVector, scalarVector128);
					Sse.Store(pDstCurrent, dstVector);

					pDstCurrent += 4;
				}

				while (pDstCurrent < pDstEnd)
				{
					var dstVector = Sse.LoadScalarVector128(pDstCurrent);
					dstVector = Sse.AddScalar(dstVector, scalarVector128);
					Sse.StoreScalar(pDstCurrent, dstVector);

					pDstCurrent++;
				}
			}
		}
#endif
		public override string GetCategory()
		{
			return "extension";
		}
	}
}