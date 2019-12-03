#region using

using System.Security.Cryptography;
using System.Threading.Tasks;

#endregion

namespace Benchmarking.Cryptography
{
	internal class CSPRNG : Benchmark
	{
		public CSPRNG(Options options) : base(options)
		{
		}

		public override void Run()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				threads[i] = Task.Run(() =>
				{
					var data = new byte[1000000000 / options.Threads];
					var csrpng = RandomNumberGenerator.Create();

					for (var j = 0; j < 64; j++)
					{
						csrpng.GetBytes(data);
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(threads);
		}

		public override string GetDescription()
		{
			return "Generates 1GB of cryptographically secure random data 64 times";
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 17329.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 4004.0d;
		}

		public override string GetCategory()
		{
			return "cryptography";
		}
	}
}