#region using

using System.IO;
using System.Threading.Tasks;
using Benchmarking.Util;
using ICSharpCode.SharpZipLib.GZip;

#endregion

namespace Benchmarking.Decompression
{
	internal class GZip : Benchmark
	{
		private readonly byte[][] datas;
		private readonly uint volume = 50000000;

		public GZip(Options options) : base(options)
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
						using var stream = new GZipInputStream(s);
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
			return "Decompressing data with GZip";
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
					using (var stream = new GZipOutputStream(s))
					{
						stream.SetLevel(9);

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
					return 864.0d;
				}
				default:
				{
					return 334.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"decompression"};
		}
	}
}