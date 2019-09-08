﻿#region using

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Parsing
{
	internal class JSONParser : Benchmark
	{
		private readonly string[] datas;

		public JSONParser(Options options) : base(options)
		{
			datas = new string[options.Threads];
		}

		public override void Run()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				threads[i1] = Task.Run(() =>
				{
					var doc = JsonConvert.DeserializeObject<Dictionary<string, Options>>(datas[i1]);

					options = doc["options0"];
				});
			}

			Task.WaitAll(threads);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 1538.0d;
			}

			return 702.0d;
		}

		public override string GetCategory()
		{
			return "parsing";
		}

		public override void Initialize()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				threads[i1] = Task.Run(() =>
				{
					var obj = new Dictionary<string, Options>();

					for (var j = 0; j < 500000 / options.Threads; j++)
					{
						obj.Add("options" + j, options);
					}

					datas[i1] = JsonConvert.SerializeObject(obj);
				});
			}

			Task.WaitAll(threads);
		}
	}
}