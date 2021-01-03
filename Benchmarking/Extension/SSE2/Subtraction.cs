using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE2
{
    public class Subtraction : BaseSse2
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse2.IsSupported)
            {
                return 0uL;
            }

            var randomIntSpan = new Span<int>(new[] {randomInt, randomInt, randomInt, randomInt});
            var dst = new Span<int>(Enumerable.Repeat(int.MaxValue, 4).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (int* pdst = dst)
                fixed (int* psrc = randomIntSpan)
                {
                    var srcVector = Sse2.LoadVector128(psrc);
                    var dstVector = Sse2.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Sse2.Subtract(dstVector, srcVector);
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
            return "SSE2 benchmark of subtraction on 128-bit ints (4 numbers)";
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
            return "sse2_sub";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse2", "subtraction", "all"};
        }
    }
}