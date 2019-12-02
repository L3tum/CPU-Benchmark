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

		public Deflate(Options options) : base(options)
		{
			datas = new byte[options.Threads][];
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
			return "Decompressing 1 GB of data with Deflate/Inflate";
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

		public override double GetReferenceValue()
		{
			return 3463.0d;
		}

		public override string GetCategory()
		{
			return "decompression";
		}
	}
}