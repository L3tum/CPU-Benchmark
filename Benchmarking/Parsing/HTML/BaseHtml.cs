namespace Benchmarking.Parsing.HTML
{
    public class BaseHtml : Benchmark
    {
        public override string[] GetCategories()
        {
            return new[] {"all", "parsing", "html"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) iterations;
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}