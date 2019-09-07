#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

#endregion

namespace Benchmarking.Compression
{
	internal class Deflate : Benchmark
	{
		private readonly string[] datas;

		public Deflate(Options options) : base(options)
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
						using (var stream = new DeflaterOutputStream(s, new Deflater(Deflater.BEST_COMPRESSION)))
						{
							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.Finish();
							}
						}
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Compressing 1 GB of data with Deflate";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString(500000000 / options.Threads); });
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 23083.0d;
			}

			return 2556.0d;
		}
		public override string GetCategory()
		{
			return "compression";
		}
	}
}