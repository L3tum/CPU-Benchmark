#region using

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#endregion

namespace Benchmarking.Arithmetic
{
	internal class Float : Benchmark
	{
		private const int LENGTH = 200000000;
		private const float randomFloat = float.Epsilon;
		private float[] floatArray;

		public Float(Options options) : base(options)
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
					return 8689.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 308.0d;
		}

		public override string GetDescription()
		{
			return "Float arithmetic performance";
		}

		public override void Initialize()
		{
			floatArray = new float[LENGTH];
		}

		public override string GetName()
		{
			return "arithmetic_float";
		}

		public override string GetCategory()
		{
			return "float";
		}
	}
}