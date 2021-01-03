using System.Threading;
using HtmlAgilityPack;

namespace Benchmarking.Parsing.HTML
{
    public class SmallHtmlFileParser : BaseHtml
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(SmallHtmlFile.FILE);

                iterations++;
            }

            return iterations;
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return base.GetDataThroughput(iterations) * SmallHtmlFile.FILE.Length;
        }
    }
}