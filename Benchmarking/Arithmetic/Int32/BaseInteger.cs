using System;

namespace Benchmarking.Arithmetic.Int32
{
    public class BaseInteger : Benchmark
    {
        protected int RandomInt;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(int) * (double) (iterations * LENGTH);
        }

        public override void Initialize()
        {
            var rand = new Random();

            RandomInt = rand.Next();
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}