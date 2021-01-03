using System;

namespace Benchmarking.Extension.AVX2
{
    public class BaseAvx2 : Benchmark
    {
        protected int randomInt;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(int) * 8 * (double) (LENGTH * iterations);
        }

        public override void Initialize()
        {
            var rand = new Random();

            randomInt = rand.Next();
        }
    }
}