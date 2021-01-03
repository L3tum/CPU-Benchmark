#region using

#endregion

using System;

namespace Benchmarking.Extension.SSE2
{
    public class BaseSse2 : Benchmark
    {
        protected int randomInt;

        public override void Initialize()
        {
            var rand = new Random();

            randomInt = rand.Next();
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(int) * 4 * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}