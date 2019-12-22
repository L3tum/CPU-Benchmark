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
	internal class AVX2Int : Benchmark
	{
		private List<uint[]> datas;
		private uint randomInteger;

		public AVX2Int(Options options) : base(options)
		{
		}

		public override void Run()
		{
#if NETCOREAPP3_0
			if (!Avx2.IsSupported)
			{
				return;
			}

			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				threads[i] = Task.Run(() =>
				{
					var randomIntegerSpan = new Span<uint>(new[] {randomInteger});
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
			return "AVX benchmark of addition and multiplication on 512 integers (2048 bits) 100.000.000 times.";
		}

		public override void Initialize()
		{
			randomInteger = 3;

			datas = new List<uint[]>((int) options.Threads);

			for (var i = 0; i < options.Threads; i++)
			{
				// Multiple of 256 to test AVX only
				datas.Add(new uint[512]);
			}
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 6536.0d;
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
			if (!Avx2.IsSupported)
			{
				return 0.0d;
			}

			return 1100.0d;
#else
			return 0.0d;
#endif
		}

		public override string[] GetCategories()
		{
			return new[] { "extension", "int" };
		}

#if NETCOREAPP3_0
		private unsafe void MultiplyScalarU(Span<uint> scalar, Span<uint> dst)
		{
			fixed (uint* pdst = dst)
			fixed (uint* psrc = scalar)
			{
				var pDstEnd = pdst + dst.Length;
				var pDstCurrent = pdst;

				var scalarVector256 = Avx2.BroadcastScalarToVector256(psrc);

				while (pDstCurrent + 8 <= pDstEnd)
				{
					var dstVector = Avx.LoadVector256(pDstCurrent);
					dstVector = Avx2.Multiply(dstVector, scalarVector256).AsUInt32();
					Avx.Store(pDstCurrent, dstVector);

					pDstCurrent += 8;
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

				var scalarVector256 = Avx2.BroadcastScalarToVector256(psrc);

				while (pDstCurrent + 8 <= pDstEnd)
				{
					var dstVector = Avx.LoadVector256(pDstCurrent);
					dstVector = Avx2.Add(dstVector, scalarVector256);
					Avx.Store(pDstCurrent, dstVector);

					pDstCurrent += 8;
				}
			}
		}
#endif
	}
}