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
		private const uint volume = 50000000;
		private readonly string[] datas;
		private readonly uint numberOfIterations = 10;

		public ZIP(Options options) : base(options)
		{
			datas = new string[options.Threads];

			numberOfIterations *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					for (var j = 0; j < numberOfIterations; j++)
					{
						using Stream s = new MemoryStream();
						using var sw = new StreamWriter(s);
						sw.Write(datas[i1]);
						sw.Flush();

						s.Seek(0, SeekOrigin.Begin);

						using var stream = new ZipInputStream(s);
						var zipEntry = stream.GetNextEntry();

						zipEntry.DateTime = DateTime.Now;
					}

					BenchmarkRunner.ReportProgress();
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
			return "Decompressing data with ZIP";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() =>
				{
					var data = DataGenerator.GenerateString((int) (volume / options.Threads));

					using Stream s = new MemoryStream();
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
					return 1966.0d;
				}
				default:
				{
					return 375.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"decompression"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume * numberOfIterations / (timeInMillis / 1000);
		}
	}
}