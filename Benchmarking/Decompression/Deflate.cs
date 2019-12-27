#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

#endregion

namespace Benchmarking.Decompression
{
	internal class Deflate : Benchmark
	{
		private readonly byte[][] datas;
		private readonly uint volume = 50000000;

		public Deflate(Options options) : base(options)
		{
			datas = new byte[options.Threads][];

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
					using (Stream s = new MemoryStream(datas[i1]))
					{
						using var stream = new InflaterInputStream(s);
						using var sr = new StreamReader(stream);
						sr.ReadToEnd();
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
			return "Decompressing 1 GB of data with Deflate/Inflate";
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

					using var s = new MemoryStream();
					using (var stream = new DeflaterOutputStream(s, new Deflater(Deflater.BEST_COMPRESSION)))
					{
						using var sw = new StreamWriter(stream);
						sw.Write(data);
						sw.Flush();
						stream.Finish();

						stream.IsStreamOwner = false;
					}

					s.Seek(0, SeekOrigin.Begin);

					datas[i1] = s.ToArray();
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
					return 689.0d;
				}
				default:
				{
					return 315.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"decompression"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume / (timeInMillis / 1000);
		}
	}
}