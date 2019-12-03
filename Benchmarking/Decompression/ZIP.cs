#region using

using System;
using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace Benchmarking.Decompression
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
						using var sw = new StreamWriter(s);
						sw.Write(datas[i1]);
						sw.Flush();

						s.Seek(0, SeekOrigin.Begin);

						using var stream = new ZipInputStream(s);
						var zipEntry = stream.GetNextEntry();

						zipEntry.DateTime = DateTime.Now;
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetName()
		{
			return base.GetName() + "-decompression";
		}

		public override string GetDescription()
		{
			return "Decompressing 1 GB of data with ZIP";
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
					// 500000000
					var data = DataGenerator.GenerateString((int) (500000000 / options.Threads));

					using (Stream s = new MemoryStream())
					{
						using (var stream = new ZipOutputStream(s))
						{
							stream.SetLevel(9);

							var entry = new ZipEntry("test.txt") {DateTime = DateTime.Now};

							stream.PutNextEntry(entry);

							using var sw = new StreamWriter(stream);
							sw.Write(data);
							sw.Flush();

							stream.CloseEntry();
							stream.IsStreamOwner = false;
						}

						s.Seek(0, SeekOrigin.Begin);

						using var sr = new StreamReader(s);
						datas[i1] = sr.ReadToEnd();
					}

					BenchmarkRunner.ReportProgress(GetName());
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
					return 1940.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 256.0d;
		}

		public override string GetCategory()
		{
			return "decompression";
		}
	}
}