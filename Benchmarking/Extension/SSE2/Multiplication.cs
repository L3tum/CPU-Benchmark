using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE2
{
    public class Multiplication : BaseSse2
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse2.IsSupported)
            {
                return 0uL;
            }

            var randomIntSpan = new Span<uint>(new[]
                {(uint) randomInt, (uint) randomInt, (uint) randomInt, (uint) randomInt});
            var dst = new Span<uint>(Enumerable.Repeat(1u, 4).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (uint* pdst = dst)
                fixed (uint* psrc = randomIntSpan)
                {
                    var srcVector = Sse2.LoadVector128(psrc);
                    var dstVector = Sse2.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Sse2.Multiply(dstVector, srcVector).As<ulong, uint>();
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
            return "SSE2 benchmark of multiplication on 128-bit ints (4 numbers)";
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
            return "sse2_mul";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse2", "multiplication", "all"};
        }
    }
}