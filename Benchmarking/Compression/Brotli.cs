#region using

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Compression
{
	[Obsolete]
	internal class Brotli : Benchmark
	{
		private readonly string[] datas;

		public Brotli(Options options) : base(options)
		{
			datas = new string[options.Threads];
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
						using (var stream = new BrotliStream(s, CompressionLevel.Optimal))
						{
							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
							}
						}
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Compressing 1 GB of data with Brotli";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString((int) (500000000 / options.Threads)); });
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 586459.0d;
			}

			return 52823.0d;
		}

		public override string GetCategory()
		{
			return "compression";
		}
	}
}