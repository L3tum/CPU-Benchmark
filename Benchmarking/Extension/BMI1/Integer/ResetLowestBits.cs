using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI1.Integer
{
    public class ResetLowestBits : BaseBmi1
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi1.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var rlsb = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    rlsb = Bmi1.ResetLowestSetBit(rlsb);
                }

                iterations++;
            }

            return iterations + rlsb - rlsb;
        }

        public override string GetDescription()
        {
            return "BMI1 benchmark of resetting the lowest set bits on 32-bit ints";
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
            return "bmi1_blsr";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi1", "blsr", "all"};
        }
    }
}