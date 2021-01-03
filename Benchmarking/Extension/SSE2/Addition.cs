using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE2
{
    public class Addition : BaseSse2
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse2.IsSupported)
            {
                return 0uL;
            }

            var randomIntSpan = new Span<int>(new[] {randomInt, randomInt, randomInt, randomInt});
            var dst = new Span<int>(new int[4]);
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
                            dstVector = Sse2.Add(dstVector, srcVector);
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
            return "SSE2 benchmark of addition on 128-bit ints (4 numbers)";
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
            return "sse2_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse2", "addition", "all"};
        }
    }
}