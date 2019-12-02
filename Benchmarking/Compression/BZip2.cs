#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.BZip2;

#endregion

namespace Benchmarking.Compression
{
	internal class BZip2 : Benchmark
	{
		private readonly string[] datas;

		public BZip2(Options options) : base(options)
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
						using var stream = new BZip2OutputStream(s);
						using var sw = new StreamWriter(stream);

						sw.Write(datas[i1]);
						sw.Flush();
						stream.Flush();
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Compressing 128 MB of data with BZip2";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 64 "MB" string -> 2 bytes per character -> 128 MB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				// 500000000
				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString((int) (64000000 / options.Threads)); });
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			return 5727.0d;
		}
		public override string GetCategory()
		{
			return "compression";
		}
	}
}