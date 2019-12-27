#region using

using System.Security.Cryptography;
using System.Threading.Tasks;

#endregion

namespace Benchmarking.Cryptography
{
	internal class CSPRNG : Benchmark
	{
		private readonly uint numberOfIterations = 4;
		private const uint volume = 1000000000;

		public CSPRNG(Options options) : base(options)
		{
			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = Task.Run(() =>
				{
					var data = new byte[volume / options.Threads];
					var csrpng = RandomNumberGenerator.Create();

					for (var j = 0; j < numberOfIterations; j++)
					{
						csrpng.GetBytes(data);
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(threads);
		}

		public override string GetDescription()
		{
			return $"Generates cryptographically secure random data {numberOfIterations} times";
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1157.0d;
				}
				default:
				{
					return 260.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"cryptography", "int"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(byte) * numberOfIterations * volume / (timeInMillis / 1000);
		}
	}
}