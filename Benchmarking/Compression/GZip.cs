#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.GZip;

#endregion

namespace Benchmarking.Compression
{
	internal class GZip : Benchmark
	{
		private readonly string[] datas;
		private readonly uint volume = 50000000;

		public GZip(Options options) : base(options)
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
				tasks[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					using (Stream s = new MemoryStream())
					{
						using var stream = new GZipOutputStream(s);
						stream.SetLevel(9);

						using var sw = new StreamWriter(stream);
						sw.Write(datas[i1]);
						sw.Flush();
						stream.Finish();
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Compressing data with GZip";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

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
					return 2560.0d;
				}
				default:
				{
					return 362.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"compression"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume / (timeInMillis / 1000);
		}
	}
}