using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSSE3
{
    public class HorizontalSubtraction : BaseSsse3
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Ssse3.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<int>(new[] {randomInt, randomInt, randomInt, randomInt});
            var dst = new Span<int>(Enumerable.Repeat(int.MaxValue / 2, 4).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (int* pdst = dst)
                fixed (int* psrc = randomFloatingSpan)
                {
                    var srcVector = Sse2.LoadVector128(psrc);
                    var dstVector = Sse2.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Ssse3.HorizontalSubtract(dstVector, srcVector);
                        }

                        Sse2.Store(pdst, dstVector);

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "SSSE3 benchmark of horizontal-subtraction on 128-bit ints (4 numbers)";
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
            return "ssse3_hsub";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "ssse3", "hsub", "all"};
        }
    }
}