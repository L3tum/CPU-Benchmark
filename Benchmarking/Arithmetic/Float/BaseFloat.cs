#region using

#endregion

namespace Benchmarking.Arithmetic.Float
{
    public class BaseFloat : Benchmark
    {
        protected const float RANDOM_FLOAT = float.Epsilon;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * (double) (LENGTH * iterations);
        }
        
        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}