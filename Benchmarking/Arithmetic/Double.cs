#region using

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#endregion

namespace Benchmarking.Arithmetic
{
	internal class Double : Benchmark
	{
		private const int LENGTH = 200000000;
		private const double randomFloat = double.Epsilon;
		private double[] floatArray;

		public Double(Options options) : base(options)
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
					return 15408.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 635.0d;
		}

		public override string GetDescription()
		{
			return "Double arithmetic performance";
		}

		public override void Initialize()
		{
			floatArray = new double[LENGTH];
		}

		public override string GetName()
		{
			return "arithmetic_double";
		}

		public override string[] GetCategories()
		{
			return new[] { "float", "arithmetic" };
		}
	}
}