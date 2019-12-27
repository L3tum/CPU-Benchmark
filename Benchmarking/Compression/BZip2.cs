#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.BZip2;

#endregion

namespace Benchmarking.Compression
{
	internal class BZip2 : Benchmark
	{
		private readonly string[] datas;
		private readonly uint volume = 6400000;

		public BZip2(Options options) : base(options)
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
						using var stream = new BZip2OutputStream(s);
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
			return "Compressing data with BZip2";
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
					return 783.0d;
				}
				default:
				{
					return 527.0d;
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