using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.PCLMULQDQ
{
    public class CarrylessMultiply : BasePclmuldqd
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Pclmulqdq.IsSupported)
            {
                return 0uL;
            }

            var randomIntSpan = new Span<long>(new[] {randomInt, randomInt});
            var dst = new Span<long>(Enumerable.Repeat(1L, 2).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (long* pdst = dst)
                fixed (long* psrc = randomIntSpan)
                {
                    var srcVector = Sse2.LoadVector128(psrc);
                    var dstVector = Sse2.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Pclmulqdq.CarrylessMultiply(dstVector, srcVector, 0b00);
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
            return "PCLMULDQD benchmark of carryless-multiply calculation on 64-bit ints (128-bit integer result)";
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
            return "pclmuldqd_cm";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "pclmuldqd", "carryless_multiply", "all"};
        }
    }
}