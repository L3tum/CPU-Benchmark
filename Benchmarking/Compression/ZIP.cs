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

					BenchmarkRunner.ReportProgress();
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

				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString((int) (500000000 / options.Threads)); });
			}

			Task.WaitAll(tasks);
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 27676.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 2675.0d;
		}

		public override string[] GetCategories()
		{
			return new[] { "compression" };
		}
	}
}