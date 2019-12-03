#region using

using System.Threading.Tasks;
using HtmlAgilityPack;

#endregion

namespace Benchmarking.Parsing
{
	internal class HTMLParser : Benchmark
	{
		private readonly string[] datas;

		public HTMLParser(Options options) : base(options)
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
					var doc = new HtmlDocument();

					doc.LoadHtml(datas[i1]);
					datas[i1] = doc.DocumentNode.OuterHtml;

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(threads);
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1027.0d;
				}
				default:
				{
					return base.GetComparison();
				}
			}
		}

		public override double GetReferenceValue()
		{
			return 1266.0d;
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
					var document = new HtmlDocument();

					for (var j = 0; j < 100000 / options.Threads; j++)
					{
						document.DocumentNode.AppendChild(document.CreateElement("input"));
						document.DocumentNode.AppendChild(document.CreateElement("div").AppendChild(document
							.CreateElement("div").AppendChild(document.CreateElement("div")
								.AppendChild(document.CreateElement("div").AppendChild(document.CreateElement("div")
									.AppendChild(document.CreateElement("div")))))));
						document.DocumentNode.AppendChild(document.CreateElement("div")
							.AppendChild(document.CreateElement("div").AppendChild(document.CreateElement("button"))));
					}

					datas[i1] = document.DocumentNode.OuterHtml;
				});
			}

			Task.WaitAll(threads);
		}
	}
}