using System.Threading;
using Benchmarking.Parsing.HTML;
using HtmlAgilityPack;

namespace Benchmarking.Parsing.HTML
{
    public class LargeHtmlFileParser : BaseHtml
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(LargeHtmlFile.FILE);

                iterations++;
            }

            return iterations;
        }
        
        public override double GetDataThroughput(ulong iterations)
        {
            return base.GetDataThroughput(iterations) * LargeHtmlFile.FILE.Length;
        }
    }
}