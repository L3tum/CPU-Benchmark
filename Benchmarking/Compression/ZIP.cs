#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace Benchmarking.Compression
{
	public class ZIP : Benchmark
	{
		private readonly string[] datas;

		public ZIP(Options options) : base(options)
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
						using (var stream = new ZipOutputStream(s))
						{
							stream.SetLevel(9);
							stream.PutNextEntry(new ZipEntry("test.txt"));

							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.CloseEntry();
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
			return "Compressing 1 GB of data with ZIP";
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
				return 24878.0d;
			}

			return 2675.0d;
		}

		public override string GetCategory()
		{
			return "compression";
		}
	}
}