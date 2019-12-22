#region using

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Arithmetic
{
	internal class FP16 : Benchmark
	{
		private const int LENGTH = 200000000;
		private static readonly Half randomFloat = Half.Epsilon;
		private Half[] floatArray;

		public FP16(Options options) : base(options)
		{
		}

		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = Task.Run(() =>
				{
					// LOAD
					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] = randomFloat;
					}

					// ADD

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] += randomFloat;
					}

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] += randomFloat;
					}

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] += randomFloat;
					}

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] += randomFloat;
					}

					// SUBTRACT

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] -= randomFloat;
					}

					// MULTIPLY

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] *= randomFloat;
					}

					// DIVIDE

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] /= randomFloat;
					}

					// MODULO

					for (var j = 0 + i1 * (LENGTH / options.Threads); j < LENGTH / options.Threads; j++)
					{
						floatArray[j] %= randomFloat;
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 22430.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 982.0d;
		}

		public override string GetDescription()
		{
			return "FP16 arithmetic performance";
		}

		public override void Initialize()
		{
			floatArray = new Half[LENGTH];
		}

		public override string GetName()
		{
			return "arithmetic_fp16";
		}

		public override string GetCategory()
		{
			return "float";
		}
	}
}