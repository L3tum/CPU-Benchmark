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
			return "Decompressing 1 GB of data with BZip2";
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

					using (var s = new MemoryStream())
					{
						using (var stream = new BZip2OutputStream(s))
						{
							using var sw = new StreamWriter(stream);
							sw.Write(data);
							sw.Flush();

							stream.IsStreamOwner = false;
						}

						s.Seek(0, SeekOrigin.Begin);

						datas[i1] = s.ToArray();
					}
				});
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 32432.0d;
			}

			return 9355.0d;
		}

		public override string GetCategory()
		{
			return "decompression";
		}
	}
}