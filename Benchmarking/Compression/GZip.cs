#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.GZip;

#endregion

namespace Benchmarking.Compression
{
	internal class GZip : Benchmark
	{
		private readonly string[] datas;

		public GZip(Options options) : base(options)
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
						using var stream = new GZipOutputStream(s);
						stream.SetLevel(9);

						using var sw = new StreamWriter(stream);
						sw.Write(datas[i1]);
						sw.Flush();
						stream.Finish();
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Compressing 1 GB of data with GZip";
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
					datas[i1] = DataGenerator.GenerateString((int) (500000000 / options.Threads));
				});
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			return 3800.0d;
		}

		public override string GetCategory()
		{
			return "compression";
		}
	}
}