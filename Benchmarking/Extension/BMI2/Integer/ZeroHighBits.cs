using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI2.Integer
{
    public class ZeroHighBits : BaseBmi2
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi2.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var zhb = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    zhb = Bmi2.ZeroHighBits(zhb, 17);
                }

                iterations++;
            }

            return iterations + zhb - zhb;
        }

        public override string GetDescription()
        {
            return "BMI2 benchmark of zeroing high bits on 32-bit ints";
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
            return "bmi2_bzhi";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi2", "bzhi", "all"};
        }
    }
}