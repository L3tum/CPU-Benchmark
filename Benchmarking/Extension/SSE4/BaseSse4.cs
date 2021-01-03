#region using

#endregion

namespace Benchmarking.Extension.SSE4
{
    public class BaseSse4 : Benchmark
    {
        protected const float RANDOM_FLOAT = float.Epsilon;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 4 * (double) (LENGTH * iterations);
        }
    }
}