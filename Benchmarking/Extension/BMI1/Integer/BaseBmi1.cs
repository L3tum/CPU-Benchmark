#region using

#endregion

using System;

namespace Benchmarking.Extension.BMI1.Integer
{
    public class BaseBmi1 : Benchmark
    {
        protected uint randomInt;

        public override void Initialize()
        {
            var rand = new Random();

            randomInt = (uint) rand.Next(int.MinValue, int.MaxValue);
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(uint) * (double) (LENGTH * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 1000;
        }
    }
}