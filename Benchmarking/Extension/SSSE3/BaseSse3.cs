using System;

namespace Benchmarking.Extension.SSSE3
{
    public class BaseSsse3 : Benchmark
    {
        protected int randomInt;

        public override void Initialize()
        {
            var rand = new Random();
            randomInt = rand.Next();
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 4 * (double) (LENGTH * iterations);
        }
    }
}