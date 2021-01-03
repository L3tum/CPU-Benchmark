using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI1.Integer
{
    public class GetMaskUpToLowestSetBit : BaseBmi1
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi1.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var gmutlsb = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    gmutlsb = Bmi1.GetMaskUpToLowestSetBit(gmutlsb);
                }

                iterations++;
            }

            return iterations + gmutlsb - gmutlsb;
        }

        public override string GetDescription()
        {
            return "BMI1 benchmark of getting a bitmask up to the lowest set bit on 32-bit ints";
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
            return "bmi1_blsmsk";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi1", "blsmsk", "all"};
        }
    }
}