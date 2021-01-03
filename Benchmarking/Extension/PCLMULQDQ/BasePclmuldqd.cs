using System;

namespace Benchmarking.Extension.PCLMULQDQ
{
    public class BasePclmuldqd : Benchmark
    {
        protected long randomInt;

        public override void Initialize()
        {
            var rand = new Random();

            randomInt = rand.Next();
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(long) * 2 * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}