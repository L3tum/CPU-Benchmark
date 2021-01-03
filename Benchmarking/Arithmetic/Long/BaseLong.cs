using System;

namespace Benchmarking.Arithmetic.Long
{
    public class BaseLong : Benchmark
    {
        protected long RandomInt;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(long) * (double) (iterations * LENGTH);
        }

        public override void Initialize()
        {
            var rand = new Random();

            RandomInt = ((long) rand.Next(int.MinValue, int.MaxValue) << 32) +
                        rand.Next(int.MinValue, int.MaxValue);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}