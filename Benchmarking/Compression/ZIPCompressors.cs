#region using

using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Compression
{
	internal class ZIPCompressors : Benchmark
	{
		private readonly Benchmark[] benchmarks = new Benchmark[4];

		// 125 MB Byte Array -> 1 GB
		private readonly string[] datas;

		public ZIPCompressors(Options options) : base(options)
		{
			datas = new string[options.Threads];
		}

		public override void Run()
		{
			foreach (var benchmark in benchmarks)
			{
				benchmark?.Run();
			}
		}

		public override string GetDescription()
		{
			return "Compressing 1 GB of data with ZIP, GZip, Deflate, BZip2";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString(500000000 / options.Threads); });
			}

			Task.WaitAll(tasks);

			benchmarks[0] = new ZIP(options, datas);
			benchmarks[1] = new BZip2(options, datas);
			benchmarks[2] = new GZip(options, datas);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 135984.0d;
			}

			return 12732.0d;
		}
	}
}