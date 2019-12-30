#region using

using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Cryptography
{
	internal class Hashing : Benchmark
	{
		private readonly string[] datas;
		private readonly uint volume = 500000000;

		public Hashing(Options options) : base(options)
		{
			datas = new string[options.Threads];

			volume *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = Task.Run(() =>
				{
					using (Stream s = new MemoryStream())
					{
						using var stream = new CryptoStream(s, SHA256.Create(), CryptoStreamMode.Write);
						using var sw = new StreamWriter(stream);
						sw.Write(datas[i1]);
						sw.Flush();
						stream.Flush();
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Hashing data with SHA256";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() =>
				{
					datas[i1] = DataGenerator.GenerateString((int) (volume / options.Threads));
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
					return 740.0d;
				}
				default:
				{
					return 100.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"cryptography", "int"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume * 2 / (timeInMillis / 1000);
		}
	}
}