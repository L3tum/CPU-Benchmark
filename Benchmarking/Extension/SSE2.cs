#region using

using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
#if NETCOREAPP3_0
using System.Runtime.Intrinsics.X86;

#endif

#endregion

namespace Benchmarking.Extension
{
	internal class SSE2 : Benchmark
	{
		private List<uint[]> datas;
		private uint randomIntegerNumber;

		public SSE2(Options options) : base(options)
		{
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
				var i1 = i;
				threads[i] = Task.Run(() =>
				{
					var randomIntegerSpan = new Span<uint>(new[] {randomIntegerNumber});
					var dst = new Span<uint>(datas[i1]);

					var iterations = 100000000 / options.Threads;

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

		public override string GetDescription()
		{
			return "SSE2 benchmark of addition and multiplication on 256 integers (1024 bits) 100.000.000 times.";
		}

		public override void Initialize()
		{
			randomIntegerNumber = 3;

			datas = new List<uint[]>((int) options.Threads);

			for (var i = 0; i < options.Threads; i++)
			{
				// Multiple of 128 to test SSE only
				datas.Add(new uint[256]);
			}
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 3596.0d;
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
			if (!Sse2.IsSupported)
			{
				return 0.0d;
			}

			return 1100.0d;
#else
			return 0.0d;
#endif
		}

		public override string GetCategory()
		{
			return "extension";
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