#region using

#endregion

using System;

namespace Benchmarking.Extension.BMI1.Long
{
    public class BaseBmi1 : Benchmark
    {
        protected ulong randomInt;

        public override void Initialize()
        {
            var rand = new Random();

            randomInt = ((ulong) rand.Next(int.MinValue, int.MaxValue) << 32) +
                        (ulong) rand.Next(int.MinValue, int.MaxValue);
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(ulong) * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}