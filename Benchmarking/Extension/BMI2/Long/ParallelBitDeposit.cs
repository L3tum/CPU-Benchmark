using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI2.Long
{
    public class ParallelBitDeposit : BaseBmi2
    {
        private ulong anotherRandomInt;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi2.X64.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var zhb = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    zhb = Bmi2.X64.ParallelBitDeposit(zhb, anotherRandomInt);
                }

                iterations++;
            }

            return iterations + zhb - zhb;
        }

        public override void Initialize()
        {
            base.Initialize();
            var rand = new Random();
            anotherRandomInt = ((ulong) rand.Next(int.MinValue, int.MaxValue) << 32) +
                               (ulong) rand.Next(int.MinValue, int.MaxValue);
        }

        public override string GetDescription()
        {
            return "BMI2 benchmark of parallel bit deposit on 64-bit ints";
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1010;
                }
                default:
                {
                    return 200;
                }
            }
        }

        public override string GetName()
        {
            return "bmi2_pdep_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi2", "pdep", "all"};
        }
    }
}