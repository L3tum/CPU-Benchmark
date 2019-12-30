#region using

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Arithmetic
{
	internal class FP16 : Benchmark
	{
		private static readonly Half randomFloat = Half.Epsilon;
		private readonly uint LENGTH = 20000000;
		private Half[] floatArray;

		public FP16(Options options) : base(options)
		{
			LENGTH *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
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
					return 2280.0d;
				}
				default:
				{
					return 96.0d;
				}
			}
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

		public override string[] GetCategories()
		{
			return new[] {"float", "arithmetic", "ml"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(float) / 2 * LENGTH * LENGTH * 8 / (timeInMillis / 1000);
		}
	}
}