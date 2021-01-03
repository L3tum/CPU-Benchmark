namespace Benchmarking.Parsing.JSON
{
    public class BaseJson : Benchmark
    {
        protected new const int LENGTH = 100;
        
        public override string[] GetCategories()
        {
            return new[] {"all", "parsing", "json"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}