#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.BZip2;

#endregion

namespace Benchmarking.Decompression
{
	internal class BZip2 : Benchmark
	{
		private readonly byte[][] datas;

		public BZip2(Options options) : base(options)
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
						using var stream = new BZip2InputStream(s);
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
			return "Decompressing 512 MB of data with BZip2";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 256 "MB" string -> 2 bytes per character -> 512 MB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() =>
				{
					// 500000000
					var data = DataGenerator.GenerateString((int) (256000000 / options.Threads));

					using var s = new MemoryStream();
					using (var stream = new BZip2OutputStream(s))
					{
						using var sw = new StreamWriter(stream);
						sw.Write(data);
						sw.Flush();

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
					return 19389.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 5104.0d;
		}

		public override string[] GetCategories()
		{
			return new[] { "decompression" };
		}
	}
}