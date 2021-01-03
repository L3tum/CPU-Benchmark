namespace Benchmarking.Extension.AVX
{
    public class BaseAvx : Benchmark
    {
        protected const float RANDOM_FLOAT = float.Epsilon;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 8 * (double) (LENGTH * iterations);
        }
    }
}