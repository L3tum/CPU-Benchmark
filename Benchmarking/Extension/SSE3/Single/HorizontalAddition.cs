using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE3.Single
{
    public class HorizontalAddition : BaseSse3
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse3.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[] {RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT});
            var dst = new Span<float>(Enumerable.Repeat(float.MaxValue / 2, 4).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (float* pdst = dst)
                fixed (float* psrc = randomFloatingSpan)
                {
                    var srcVector = Sse.LoadVector128(psrc);
                    var dstVector = Sse.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Sse3.HorizontalAdd(dstVector, srcVector);
                        }

                        Sse.Store(pdst, dstVector);

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "SSE3 benchmark of horizontal-addition on 128-bit floats (4 numbers)";
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
            return "sse3_hadd_single";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse3", "hadd", "all"};
        }
    }
}