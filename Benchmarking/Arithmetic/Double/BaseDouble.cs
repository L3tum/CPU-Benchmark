#region using

#endregion

namespace Benchmarking.Arithmetic.Double
{
    public class BaseDouble : Benchmark
    {
        protected const double RANDOM_DOUBLE = double.Epsilon;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(double) * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}