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
				});
			}

			Task.WaitAll(threads);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 7371.0d;
			}

			return 7776.0d;
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

					for (var j = 0; j < 500000 / options.Threads; j++)
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