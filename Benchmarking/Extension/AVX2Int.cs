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
		private readonly uint numberOfIterations = 10000000u;
		private List<uint[]> datas;
		private uint randomInteger;

		public AVX2Int(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
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

		public override string GetDescription()
		{
			return "AVX benchmark of addition and multiplication on 512 integers (16384 bits)";
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
					return 564.0d;
				}
				default:
				{
					return 125.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"extension", "int"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(uint) * 512 * numberOfIterations * 2 / (timeInMillis / 1000);
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