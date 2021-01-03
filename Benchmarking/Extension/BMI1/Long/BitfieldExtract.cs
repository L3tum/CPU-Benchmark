using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI1.Long
{
    public class BitfieldExtract : BaseBmi1
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi1.X64.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var bfe = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    bfe = Bmi1.X64.BitFieldExtract(bfe, 0, 16);
                }

                iterations++;
            }

            return iterations + bfe - bfe;
        }

        public override string GetDescription()
        {
            return "BMI1 benchmark of bitfield-extraction on 64-bit ints";
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
            return "bmi1_bextr_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi1", "bextr", "all"};
        }
    }
}