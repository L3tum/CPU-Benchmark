namespace Benchmarking.Extension.SSE3.Single
{
    public class BaseSse3 : Benchmark
    {
        protected const float RANDOM_FLOAT = float.Epsilon;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 4 * (double) (LENGTH * iterations);
        }
    }
}